using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace HPM.Web.Localization
{
    public class I18nService
    {
        public const string EnglishCode = "en";
        public const string BanglaCode = "bn";

        private string _currentLanguage = EnglishCode;
        private bool _isInitialized = false;

        public string CurrentLanguage => _currentLanguage;
        public bool IsBangla => _currentLanguage == BanglaCode;
        public bool IsEnglish => _currentLanguage == EnglishCode;
        public string CurrencySymbol => IsBangla ? "৳" : "Tk";

        public event Action? OnLanguageChanged;

        public async Task InitializeAsync(IJSRuntime js)
        {
            if (_isInitialized) return;
            try
            {
                var storedLang = await js.InvokeAsync<string>("localStorage.getItem", "hpm_language");
                if (!string.IsNullOrWhiteSpace(storedLang) && (storedLang == BanglaCode || storedLang == EnglishCode))
                {
                    _currentLanguage = storedLang;
                }
                _isInitialized = true;
            }
            catch
            {
                // LocalStorage might be inaccessible during pre-rendering or SSR
                _isInitialized = true;
            }
        }

        public async Task SetLanguageAsync(string languageCode, IJSRuntime? js = null)
        {
            if (string.IsNullOrWhiteSpace(languageCode)) return;
            languageCode = languageCode.ToLowerInvariant();
            if (languageCode != EnglishCode && languageCode != BanglaCode)
            {
                languageCode = EnglishCode;
            }

            if (_currentLanguage != languageCode)
            {
                _currentLanguage = languageCode;
                if (js != null)
                {
                    try
                    {
                        await js.InvokeVoidAsync("localStorage.setItem", "hpm_language", _currentLanguage);
                    }
                    catch
                    {
                        // Ignore JS interop errors if offline or during teardown
                    }
                }
                if (OnLanguageChanged != null)
                {
                    foreach (var handler in OnLanguageChanged.GetInvocationList())
                    {
                        try
                        {
                            ((Action)handler)();
                        }
                        catch
                        {
                            // Ignore disposed components or transient state errors
                        }
                    }
                }
            }
        }

        public async Task ToggleLanguageAsync(IJSRuntime? js = null)
        {
            var next = IsBangla ? EnglishCode : BanglaCode;
            await SetLanguageAsync(next, js);
        }

        /// <summary>
        /// Translate a string key into the current active language (fallback to English if not found).
        /// </summary>
        public string T(string? key, string? defaultEnglish = null)
        {
            if (string.IsNullOrEmpty(key)) return defaultEnglish ?? string.Empty;
            var text = TranslationDictionary.GetTranslation(key, _currentLanguage);
            if (string.IsNullOrEmpty(text) && defaultEnglish != null)
            {
                return defaultEnglish;
            }
            return text;
        }

        public string this[string? key] => T(key);

        /// <summary>
        /// Formats currency with appropriate symbol (e.g. ৳ or Tk / $)
        /// </summary>
        public string FormatCurrency(decimal amount)
        {
            if (IsBangla)
            {
                return $"৳ {amount:N2}";
            }
            return $"Tk {amount:N2}";
        }

        /// <summary>
        /// Format date according to culture preference
        /// </summary>
        public string FormatDate(DateTime date, string format = "dd MMM yyyy")
        {
            return date.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Optional: convert western digits (0-9) to Bangla digits (০-৯)
        /// </summary>
        public string ToBanglaDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input
                .Replace('0', '০')
                .Replace('1', '১')
                .Replace('2', '২')
                .Replace('3', '৩')
                .Replace('4', '৪')
                .Replace('5', '৫')
                .Replace('6', '৬')
                .Replace('7', '৭')
                .Replace('8', '৮')
                .Replace('9', '৯');
        }
    }
}
