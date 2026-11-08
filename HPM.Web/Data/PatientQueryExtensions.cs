using HPM.Web.Data.Entities;

namespace HPM.Web.Data;

public static class PatientQueryExtensions
{
    /// <summary>
    /// Projects all Registration columns except the large Base64 PhotoData (nvarchar(max)) column.
    /// Use this on all search, autocomplete, dropdown, and list queries where the patient photo is not rendered.
    /// </summary>
    public static IQueryable<Registration> SelectWithoutPhoto(this IQueryable<Registration> query) =>
        query.Select(x => new Registration
        {
            ID = x.ID,
            PatientID = x.PatientID,
            Name = x.Name,
            DateOfBirth = x.DateOfBirth,
            Sex = x.Sex,
            BloodGroup = x.BloodGroup,
            Weight = x.Weight,
            FatherName = x.FatherName,
            MotherName = x.MotherName,
            CareOf = x.CareOf,
            PermanentVillage = x.PermanentVillage,
            PermanentPO = x.PermanentPO,
            PermanentPS = x.PermanentPS,
            PermanentDistrict = x.PermanentDistrict,
            PresentVillage = x.PresentVillage,
            PresentPO = x.PresentPO,
            PresentPS = x.PresentPS,
            PresentDistrict = x.PresentDistrict,
            Contact = x.Contact,
            Email = x.Email,
            Occupation = x.Occupation,
            EnteredBy = x.EnteredBy,
            TransDate = x.TransDate
        });
}
