$connStr = "Server=np:\\.\pipe\MSSQL$HPMDB\sql\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT UserName, DisplayName, IsActive, MustChangePassword, PasswordHash FROM AspNetUsers"
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Host "$($reader['UserName']) | $($reader['DisplayName']) | Active:$($reader['IsActive']) | MustChange:$($reader['MustChangePassword'])"
}
$conn.Close()
