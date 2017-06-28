Public Class Form1
    Dim sp As String

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        MessageBox.Show("A converter for CJK_G." & vbCrLf & vbCrLf & _
                        "1、准备excel文件；" & vbCrLf & "2、将xls转换为html；" & vbCrLf & _
                        "3、建议用IE 10（windows 8）打开html并打印（acrobat）为PDF(横向上下13mm)。" & vbCrLf & _
                        vbCrLf & _
                        "Version 0.1.0.5, (c) 2012, CESI, zzx" & vbCrLf & _
                        "2012-12-07, 昆山;" & vbCrLf & _
                        "2014-06-06, 亦庄;" & vbCrLf & _
                        "2015-11-06, 雍和宫;" & vbCrLf & _
                        "2016-06-29, 远望楼.", _
                        "about www.cesi.cn/info/chinesegroup/CJK_G", _
                        MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, _
                        MessageBoxOptions.DefaultDesktopOnly, False)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtSourcePath.Text = Application.StartupPath & "\"
        sp = Application.StartupPath & "\"
        TextBox3.Text = loadConfigFile(sp & "heaven.config.txt", TextBox3.Text)
        TextBox6.Text = loadConfigFile(sp & "header.config.txt", TextBox6.Text)
        TextBox4.Text = loadConfigFile(sp & "mid.config.txt", TextBox4.Text)
        TextBox7.Text = loadConfigFile(sp & "tail.config.txt", TextBox7.Text)
        TextBox8.Text = loadConfigFile(sp & "except.config.txt", TextBox8.Text)
        TextBox9.Text = loadConfigFile(sp & "split.config.txt", TextBox9.Text)
        TextBox11.Text = loadConfigFile(sp & "page.config.txt", TextBox11.Text)
        TextBox10.Text = loadConfigFile(sp & "line.config.txt", TextBox10.Text)
        TextBox5.Text = loadConfigFile(sp & "earth.config.txt", TextBox5.Text)

        saveConfigFile(sp & "heaven.config.txt", TextBox3.Text, False)
        saveConfigFile(sp & "header.config.txt", TextBox6.Text, False)
        saveConfigFile(sp & "mid.config.txt", TextBox4.Text, False)
        saveConfigFile(sp & "tail.config.txt", TextBox7.Text, False)
        saveConfigFile(sp & "except.config.txt", TextBox8.Text, False)
        saveConfigFile(sp & "split.config.txt", TextBox9.Text, False)
        saveConfigFile(sp & "page.config.txt", TextBox11.Text, False)
        saveConfigFile(sp & "line.config.txt", TextBox10.Text, False)
        saveConfigFile(sp & "earth.config.txt", TextBox5.Text, False)
    End Sub

    Private Sub SaveUTF8(ByVal sInput As String, ByVal sOutputFile As String)
        Dim FileToWrite As System.IO.FileStream = System.IO.File.Create(sOutputFile)
        Dim abOutput() As Byte = System.Text.Encoding.UTF8.GetBytes(sInput)
        FileToWrite.Write(abOutput, 0, abOutput.Length)
        FileToWrite.Close()
        FileToWrite = Nothing
    End Sub

    Private Sub btnLoadSource_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadSource.Click
        txtSourcePath.Text = Trim(txtSourcePath.Text)
        If txtSourcePath.Text.Substring(txtSourcePath.Text.Length - 1, 1) <> "\" Then
            txtSourcePath.Text = txtSourcePath.Text & "\"
        End If
        Dim strConn As String = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + txtSourcePath.Text + txtSourceFile.Text + ";" + "Extended Properties=Excel 8.0;"
        Dim conn As New OleDb.OleDbConnection
        Dim schemaTable As DataTable

        conn.ConnectionString = strConn
        conn.Open()
        schemaTable = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, Nothing)
        lstSourceSheets.Items.Clear()
        For i = 0 To schemaTable.Rows.Count - 1
            lstSourceSheets.Items.Add(schemaTable.Rows(i).Item(2).ToString)
        Next
        conn.Close()
    End Sub

    Private Function DatabaseOpen(ByVal strConn As String, ByVal strTableName As String) As DataSet
        Dim strExcel = "select * from [" + strTableName + "]"
        Dim myCommand As New OleDb.OleDbDataAdapter(strExcel, strConn)
        Dim ds = New DataSet()
        myCommand.Fill(ds, "table1")
        DatabaseOpen = ds
    End Function

    Private Sub btnGenerateOutput_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerateOutput.Click
        Dim dataname As String

        Dim sDest As String
        Dim sOut As String
        Dim sPage As String
        Dim iline As Integer
        Dim iPageMaxline As Integer
        Dim iPageMax As Integer
        Dim iPage As Integer
        Dim i, j, l As Integer
        Dim s As String
        Dim sTemp As String
        Dim ex As Exception
        Dim sCurrCell As String

        Dim dMap(100) As String

        Dim da As String

        '0   1     2   3      4  5   6       7   8       9   10      11    12        13
        'bmp,CJK_F,kxi,stroke,fs,ghz,gsource,jhz,jsource,khz,ksource,sathz,satsource,disscusion
        Dim bmp, kxi As String
        

        Dim strConn As String = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + txtSourcePath.Text + txtSourceFile.Text + ";" + "Extended Properties=Excel 8.0;"
        Dim strExcel As String
        Dim ds As DataSet

        lstOutputFiles.Items.Clear()

        For i = 0 To lstSourceSheets.CheckedItems.Count - 1
            dataname = lstSourceSheets.CheckedItems(i)
            da = dataname.Replace("#", ".").Replace("$", "").Replace("'", "")
            strExcel = "select * from [" + dataname + "]"

            Dim myCommand As New OleDb.OleDbDataAdapter(strExcel, strConn)
            ds = New DataSet()
            myCommand.Fill(ds, "table1")

            'ds = DatabaseOpen(strConn, dataname)

            sDest = ""
            For j = 0 To ds.Tables(0).Columns.Count - 1
                '0   1     2   3      4  5   6       7   8       9   10      11    12        13
                'bmp,CJK_F,kxi,stroke,fs,ghz,gsource,jhz,jsource,khz,ksource,sathz,satsource,disscusion
                sCurrCell = ds.Tables(0).Columns(j).Caption.Replace("#", ".").Replace("$", "").Replace("'", "")
                If InStr(TextBox8.Text & vbCrLf, sCurrCell & vbCrLf) <= 0 Then
                    dMap(j) = sCurrCell
                Else
                    dMap(j) = ""
                End If
            Next

            sOut = ""

            iline = 0
            iPage = 1
            iPageMaxline = Int(TextBox10.Text)
            iPageMax = Int(TextBox11.Text)
            prcGenerate.Maximum = ds.Tables(0).Rows.Count / iPageMaxline
            prcGenerate.Value = 0
            prcGenerate.Visible = True
            j = 1
            sPage = ""
            sTemp = ""
            bmp = ""


            For j = 0 To ds.Tables(0).Rows.Count - 1
                lblLine.Text = j
                sDest = TextBox4.Text
                Application.DoEvents()
                bmp = ""
                kxi = ""

                For k = ds.Tables(0).Columns.Count - 1 To 0 Step -1
                    If dMap(k) <> "" Then
                        sCurrCell = getCellValue(ds.Tables(0).Rows(j).Item(k))
                        If InStr(dMap(k), " Image") > 0 And sCurrCell <> "" Then
                            bmp = sCurrCell
                        End If
                        For l = 1 To Len(sCurrCell)
                            If (AscW(Mid(sCurrCell, l, 1))) > 256 Then
                                txtOutputMessage.Text = txtOutputMessage.Text & vbCrLf & "Page" & iPage & ",line" & iline & ", bichar" & Mid(sCurrCell, l, 1) & " at '" & dMap(k) & "'"
                            ElseIf (AscW(Mid(sCurrCell, l, 1))) > 65535 Then
                                txtOutputMessage.Text = txtOutputMessage.Text & vbCrLf & "Page" & iPage & ",line" & iline & ", exchar" & Mid(sCurrCell, l, 1) & " at '" & dMap(k) & "'"
                            End If
                        Next

                        If InStr(dMap(k).ToLower, "kangxi radical") > 0 Then
                            kxi = Val(sCurrCell)
                        End If

                        sDest = sDest.Replace("[" & dMap(k) & "]", sCurrCell)
                    End If
                Next

                ' ext HTML
                sDest = sDest.Replace("[bmp]", bmp)
                Try
                    sDest = sDest.Replace("[kangxiRadical]", Trim(Str(Int(kxi) + 12031)))
                Catch ex
                    txtOutputMessage.Text = txtOutputMessage.Text & vbCrLf & "Err on page" & iPage & ",line" & iline & ", kxi=" & kxi
                End Try
                sDest = sDest.Replace("<img src=""img/"" width=""32"" height=""32"" border=""0"">", "")

                iline = iline + 1
                sOut = sOut & sDest
                If iline >= iPageMaxline Then
                    ' to HTML
                    sPage = sPage & Replace(TextBox6.Text, "[title]", da) & sOut & TextBox7.Text

                    iPage = iPage + 1
                    If j Mod iPageMax <> 0 Then
                        ' to HTML
                        sPage = sPage & TextBox9.Text.Replace("[page]", iPage - 1)
                    End If
                    sOut = ""
                    If iPage Mod iPageMax = 0 Then

                        ' to HTML
                        SaveUTF8(TextBox3.Text & sPage & TextBox5.Text, txtSourcePath.Text & da & Int(j / iPageMax) & ".html")
                        sPage = ""

                    End If
                    iline = 0
                    prcGenerate.Value = prcGenerate.Value + 1
                End If

            Next

            If iline >= 1 Then
                s = ""
                For j = Int(TextBox10.Text) To iline Step -1
                    s = s & "<p/>&nbsp;"
                Next
                sPage = sPage & Replace(TextBox6.Text, "[title]", da) & sOut & TextBox7.Text & s & TextBox9.Text.Replace("[page]", iPage)
            End If

            SaveUTF8(Replace(TextBox3.Text, "[title]", da) & sPage & TextBox5.Text, txtSourcePath.Text & da & ".html")
            lstOutputFiles.Items.Add(txtSourcePath.Text & da & ".html")
            prcGenerate.Visible = False

        Next
    End Sub

    Function getCellValue(ByVal itemCell) As String
        If IsDBNull(itemCell) Then
            getCellValue = ""
        Else
            getCellValue = itemCell
        End If
    End Function

    Private Sub PrintDocument1_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Dim linePen As New Pen(Color.Black)
        linePen.Width = 1
        '300像素/英寸，a4大小是2479x3508
        '120像素/英寸，a4大小是1487x2105
        'linePen.Color = Color.Black
        e.Graphics.DrawLine(linePen, 20, 20, 1280, 20)
        e.Graphics.DrawLine(linePen, 20, 40, 270, 40)
        e.Graphics.DrawLine(linePen, 20, 60, 270, 60)
    End Sub

    Private Sub btnGenerateOFD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerateOFD.Click
        'PrintDocument1.Print()
        PrintDocument1.DefaultPageSettings.Landscape = True
        'PrintDocument1.DefaultPageSettings.PaperSize.PaperName = "A4"
        PrintPreviewDialog1.Document = PrintDocument1
        PrintPreviewDialog1.ShowDialog()
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        saveConfigFile(sp & "heaven.config.txt", TextBox3.Text, True)
        saveConfigFile(sp & "header.config.txt", TextBox6.Text, True)
        saveConfigFile(sp & "mid.config.txt", TextBox4.Text, True)
        saveConfigFile(sp & "tail.config.txt", TextBox7.Text, True)
        saveConfigFile(sp & "except.config.txt", TextBox8.Text, True)
        saveConfigFile(sp & "split.config.txt", TextBox9.Text, True)
        saveConfigFile(sp & "page.config.txt", TextBox11.Text, True)
        saveConfigFile(sp & "line.config.txt", TextBox10.Text, True)
        saveConfigFile(sp & "earth.config.txt", TextBox5.Text, True)
    End Sub

    Private Function loadConfigFile(ByVal filename As String, ByVal content As String) As String
        If IO.File.Exists(filename) = True Then
            loadConfigFile = IO.File.ReadAllText(filename)
        Else
            loadConfigFile = content
        End If
    End Function

    Private Sub saveConfigFile(ByVal filename As String, ByVal content As String, ByVal isM As Boolean)
        On Error GoTo errSave
        If isM = True Then
            If IO.File.Exists(filename) = True Then
                IO.File.Delete(filename)
                IO.File.WriteAllText(filename, content)
            Else
                IO.File.WriteAllText(filename, content)
            End If
        Else
            If IO.File.Exists(filename) = False Then
                IO.File.WriteAllText(filename, content)
            End If
        End If
            Exit Sub
errSave:
            MessageBox.Show("error save")
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        TextBox3.Text = loadConfigFile(sp & "heaven.config.txt", TextBox3.Text)
        TextBox6.Text = loadConfigFile(sp & "header.config.txt", TextBox6.Text)
        TextBox4.Text = loadConfigFile(sp & "mid.config.txt", TextBox4.Text)
        TextBox7.Text = loadConfigFile(sp & "tail.config.txt", TextBox7.Text)
        TextBox8.Text = loadConfigFile(sp & "except.config.txt", TextBox8.Text)
        TextBox9.Text = loadConfigFile(sp & "split.config.txt", TextBox9.Text)
        TextBox11.Text = loadConfigFile(sp & "page.config.txt", TextBox11.Text)
        TextBox10.Text = loadConfigFile(sp & "line.config.txt", TextBox10.Text)
        TextBox5.Text = loadConfigFile(sp & "earth.config.txt", TextBox5.Text)
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        saveConfigFile(sp & "test.html", TextBox3.Text & TextBox6.Text & TextBox4.Text & TextBox7.Text & TextBox9.Text & TextBox5.Text, True)
    End Sub

    Private Sub txtSourceFile_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSourceFile.GotFocus
        Me.AcceptButton = btnLoadSource
    End Sub

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        lstOutputFiles.Items.Clear()
        txtOutputMessage.Text = ""
    End Sub

    Private Sub lstSourceSheets_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstSourceSheets.GotFocus
        Me.AcceptButton = btnGenerateOutput
    End Sub

End Class
