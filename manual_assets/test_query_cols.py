import subprocess

def test_sql():
    script = """
    SELECT t.name AS TableName, c.name AS ColumnName
    FROM sys.tables t
    INNER JOIN sys.columns c ON t.object_id = c.object_id
    WHERE t.name NOT LIKE '__EF%'
    ORDER BY t.name, c.column_id
    """
    # Use sqlcmd or powershell
    ps_cmd = f"""
    $conn = New-Object Microsoft.Data.SqlClient.SqlConnection("Server=np:\\\\.\\pipe\\MSSQL`$HPMDB\\sql\\query;Database=HPM_Web;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=30")
    $conn.Open()
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "{script}"
    $r = $cmd.ExecuteReader()
    while($r.Read()){{
        Write-Output ($r['TableName'] + '.' + $r['ColumnName'])
    }}
    $conn.Close()
    """
    res = subprocess.run(["powershell", "-Command", ps_cmd], capture_output=True, text=True)
    print("Output lines:", len(res.stdout.strip().splitlines()))
    print(res.stdout[:500])

if __name__ == "__main__":
    test_sql()
