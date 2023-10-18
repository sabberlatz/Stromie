Imports Microsoft.VisualBasic.FileIO
Imports System
Imports System.Data
Imports MySql.Data.MySqlClient
Imports System.Windows.Forms

Public Class Form1
    Dim counter As Int32
    Private connectionString As String = "server=Cloud;user id=admin'@'2NBN1J3.fritz.box;password=Windows7/;database=Stromie"
    Private connection As MySqlConnection = New MySqlConnection(connectionString)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadData()
    End Sub

    Private Sub LoadData()
        Dim query As String = "SELECT ID, DATE_FORMAT(Uhrzeit, '%Y-%m-%d %H:%i') as Uhrzeit, Netzbezug, Netzeinspeisung, Stromverbrauch, Akkubeladung, Akkuentnahme, Stromerzeugung, AkkuSpannung, AkkuStromstaerke FROM Stromwerte"
        Dim adapter As MySqlDataAdapter = New MySqlDataAdapter(query, connection)
        Dim table As DataTable = New DataTable()
        adapter.Fill(table)
        DataGridView1.DataSource = table
        UpdateStatus()
    End Sub

    Private Sub UpdateStatus()
        connection.Open()
        Dim query2 As String = "SELECT COUNT(*) FROM Stromwerte"
        Using command As New MySqlCommand(query2, connection)
            Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
            ToolStripStatusLabel1.Text = "Datensätze: " & count
        End Using
        connection.Close()
    End Sub

    Private Sub ButtonCreate_Click_1(sender As Object, e As EventArgs) Handles ButtonCreate.Click
        connection.Open()
        ' Daten in die Datenbank einfügen
        Dim command As New MySqlCommand("INSERT INTO Stromwerte (ID, Uhrzeit, Netzbezug, Netzeinspeisung, Stromverbrauch, Akkubeladung, Akkuentnahme, Stromerzeugung, AkkuSpannung, AkkuStromstaerke) VALUES (uniqueId, @Wert1, @Wert2, @Wert3, @Wert4, @Wert5, @Wert6, @Wert7, @Wert8, @Wert9)", connection)
        command.Parameters.AddWithValue("@Wert1", TextBox1.Text)
        command.Parameters.AddWithValue("@Wert2", TextBox2.Text)
        command.Parameters.AddWithValue("@Wert3", TextBox3.Text)
        command.Parameters.AddWithValue("@Wert4", TextBox4.Text)
        command.Parameters.AddWithValue("@Wert5", TextBox5.Text)
        command.Parameters.AddWithValue("@Wert6", TextBox6.Text)
        command.Parameters.AddWithValue("@Wert7", TextBox7.Text)
        command.Parameters.AddWithValue("@Wert8", TextBox8.Text)
        command.Parameters.AddWithValue("@Wert9", TextBox9.Text)
        command.ExecuteNonQuery()
        ' Verbindung zur Datenbank schließen
        connection.Close()
        LoadData()
    End Sub

    Private Sub ButtonDelete_Click_1(sender As Object, e As EventArgs) Handles ButtonDelete.Click
        connection.Open()
        ' Daten in die Datenbank aktualisieren
        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim command As New MySqlCommand("DELETE FROM Stromwerte WHERE ID = @ID", connection)
        command.Parameters.AddWithValue("@ID", selectedRow.Cells("ID").Value)
        command.ExecuteNonQuery()
        ' Verbindung zur Datenbank schließen
        connection.Close()
        LoadData()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Dim connectionString As String = "Server=your_server;Database=your_database;Uid=your_username;Pwd=your_password;"
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "CSV-Dateien (*.csv)|*.csv"
        openFileDialog.Title = "Wählen Sie eine CSV-Datei aus"
        counter = 0
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Dim csvFilePath As String = openFileDialog.FileName
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Using parser As New TextFieldParser(csvFilePath)
                    parser.TextFieldType = FieldType.Delimited
                    parser.SetDelimiters(";")
                    While Not parser.EndOfData
                        Dim fields As String() = parser.ReadFields()
                        If fields.Length = 9 Then
                            counter = counter + 1
                            Dim uniqueId As String = Guid.NewGuid().ToString()
                            Dim query As String = "INSERT INTO Stromwerte (ID, Uhrzeit, Netzbezug, Netzeinspeisung, Stromverbrauch, Akkubeladung, Akkuentnahme, Stromerzeugung, AkkuSpannung, AkkuStromstaerke) VALUES (@column10, @column1, @column2, @column3, @column4, @column5, @column6, @column7, @column8, @column9)"
                            Using command As New MySqlCommand(query, connection)
                                command.Parameters.AddWithValue("@column1", fields(0))
                                command.Parameters.AddWithValue("@column2", fields(1))
                                command.Parameters.AddWithValue("@column3", fields(2))
                                command.Parameters.AddWithValue("@column4", fields(3))
                                command.Parameters.AddWithValue("@column5", fields(4))
                                command.Parameters.AddWithValue("@column6", fields(5))
                                command.Parameters.AddWithValue("@column7", fields(6))
                                command.Parameters.AddWithValue("@column8", fields(7))
                                command.Parameters.AddWithValue("@column9", fields(8))
                                command.Parameters.AddWithValue("@column10", uniqueId)
                                command.ExecuteNonQuery()
                            End Using
                        End If
                    End While
                End Using
            End Using
            UpdateStatus()
            Dim showmessage As String = "CSV-Datei erfolgreich mit " + counter.ToString() + " Einträgen in die MySQL-Datenbank importiert!"
            MessageBox.Show(showmessage)
        End If

    End Sub


End Class