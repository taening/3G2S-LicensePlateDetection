Imports System.Collections.ArrayList
Imports System.Collections.Stack
Imports Microsoft.VisualBasic.FileSystem

Public Class Form1
    Dim refer_img(9) As Bitmap
    Dim number_img(10) As Bitmap

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        'Initialize PictureBox
        picOrigin.Image = Nothing
        picProcess1.Image = Nothing
        picProcess2.Image = Nothing
        picProcess3.Image = Nothing
        picProcess4.Image = Nothing
        picProcess5.Image = Nothing
        PictureBox1.Image = Nothing
        PictureBox2.Image = Nothing
        PictureBox3.Image = Nothing
        PictureBox4.Image = Nothing
        PictureBox5.Image = Nothing
        PictureBox6.Image = Nothing
        PictureBox7.Image = Nothing

        'Initialize TextBox
        TextBox1.Text = Nothing
        TextBox2.Text = Nothing
        TextBox3.Text = Nothing
        TextBox4.Text = Nothing
        TextBox5.Text = Nothing
        TextBox6.Text = Nothing
        TextBox7.Text = Nothing


        Dim ip As New Interpolation
        Dim file As String
        Dim openFileDialog1 As New OpenFileDialog()
        Dim path As String
        path = My.Computer.FileSystem.CombinePath(My.Computer.FileSystem.CurrentDirectory, "..\..\img\test_img")
        openFileDialog1.InitialDirectory = path
        openFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"
        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            file = openFileDialog1.FileName()
            picOrigin.Image = ip.ipInterpolation(Image.FromFile(file), picOrigin.Width, picOrigin.Height, -0.5)
        End If

        refer_img(0) = New Bitmap(ip.ipInterpolation(picZero.Image, 10, 20, -0.5))
        refer_img(1) = New Bitmap(ip.ipInterpolation(picOne.Image, 10, 20, -0.5))
        refer_img(2) = New Bitmap(ip.ipInterpolation(picTwo.Image, 10, 20, -0.5))
        refer_img(3) = New Bitmap(ip.ipInterpolation(picThree.Image, 10, 20, -0.5))
        refer_img(4) = New Bitmap(ip.ipInterpolation(picFour.Image, 10, 20, -0.5))
        refer_img(5) = New Bitmap(ip.ipInterpolation(picFive.Image, 10, 20, -0.5))
        refer_img(6) = New Bitmap(ip.ipInterpolation(picSix.Image, 10, 20, -0.5))
        refer_img(7) = New Bitmap(ip.ipInterpolation(picSeven.Image, 10, 20, -0.5))
        refer_img(8) = New Bitmap(ip.ipInterpolation(picEight.Image, 10, 20, -0.5))
        refer_img(9) = New Bitmap(ip.ipInterpolation(picNine.Image, 10, 20, -0.5))
    End Sub

    Private Sub btnDraw_Click(sender As Object, e As EventArgs) Handles btnDraw.Click
        Dim pic_image As New Bitmap(picOrigin.Image)
        Dim pic_result As New Bitmap(pic_image.Width, pic_image.Height)
        Dim pic_Wlabel As New Bitmap(pic_image.Width, pic_image.Height)
        Dim pic_Blabel As New Bitmap(pic_image.Width, pic_image.Height)
        Dim pic_number As New Bitmap(pic_image.Width, pic_image.Height)
        Dim labelW_list, labelB_list, result_list, number_list As New ArrayList

        'g = Grayscale객체, t = Threshold객체, l = labeling객체, i = Interpolation객체, c = Canny객체
        Dim gray As New GrayScale() : Dim blur As New GaussianBlur() : Dim th As New Threshold() : Dim labelW, labelB As New Labeling() : Dim labelnW, labelnB As New Labeling_nonline()
        Dim mp As New Morphology() : Dim ip As New Interpolation() : Dim canny As New Canny() : Dim coner As New ConerDetection()
        Dim ls As New LicenseSearch() : Dim he As New HistogramEqualization() : Dim at As New AdabtiveThreshold()

        'Step1: Show preprocessing image to picProcess1 (GrayScale -> GaussianBlur -> Threshold)
        pic_result = gray.ipGrayScale(pic_image)
        pic_result = blur.ipGaussianBlur(pic_result, 3)
        pic_result = th.ipThresholding(pic_result)
        picProcess1.Image = pic_result

        'Step2: Show preprocessing image to picProcess2 (White area labeling)
        pic_Wlabel = labelW.ipLabeling(pic_result, "W")
        labelW_list = labelW.getLabelList()
        picProcess2.Image = pic_Wlabel

        'Step3: Show preprocessing image to picProcess3 (Black area labeling)
        pic_Blabel = labelB.ipLabeling(pic_result, "B")
        labelB_list = labelB.getLabelList()
        picProcess3.Image = pic_Blabel

        'Step4:
        Dim width, height As Integer
        Try
            result_list = ls.ipLicenseSearch(pic_image, labelW_list, labelB_list)
            Width = result_list(0)(2) - result_list(0)(0)
            Height = result_list(0)(3) - result_list(0)(1)
        Catch exception As ArgumentOutOfRangeException
            MsgBox("번호판 검출실패")
            Exit Sub
        End Try

        Dim result As New Bitmap(width + 1, height + 1)
        For y = result_list(0)(1) To result_list(0)(3)
            For x = result_list(0)(0) To result_list(0)(2)
                result.SetPixel(x - result_list(0)(0), y - result_list(0)(1), Color.FromArgb(pic_result.GetPixel(x, y).R, pic_result.GetPixel(x, y).G, pic_result.GetPixel(x, y).B))
            Next
        Next
        picProcess4.Image = result

        'Step5: 
        pic_number = labelnB.ipLabeling(result, "B")
        number_list = labelnB.getLabelList()
        picProcess5.Image = pic_number

        '여기서부터 다시 작은글자 라벨링
        Dim number(6) As Bitmap

        For i = 0 To number.Length - 1
            width = number_list(i)(2) - number_list(i)(0)
            height = number_list(i)(3) - number_list(i)(1)
            Dim result2 As New Bitmap(width + 1, height + 1)
            For x = number_list(i)(0) To number_list(i)(2)
                For y = number_list(i)(1) To number_list(i)(3)
                    result2.SetPixel(x - number_list(i)(0), y - number_list(i)(1), Color.FromArgb(result.GetPixel(x, y).R, result.GetPixel(x, y).G, result.GetPixel(x, y).B))
                Next
            Next
            number(i) = result2
        Next

        PictureBox1.Image = number(0)
        PictureBox2.Image = number(1)
        PictureBox3.Image = number(2)
        PictureBox4.Image = number(3)
        PictureBox5.Image = number(4)
        PictureBox6.Image = number(5)
        PictureBox7.Image = number(6)

        For i = 0 To number.Length - 1
            number(i) = ip.ipInterpolation(number(i), 10, 20, -0.5)
        Next

        Dim distance(9) As Integer
        Dim license_number(6) As Integer

        For i = 0 To 6
            For j = 0 To 9
                distance(j) = 0
            Next
            For y = 0 To number(i).Height - 1
                For x = 0 To number(i).Width - 1

                    For j = 0 To 9
                        If number(i).GetPixel(x, y).R <> refer_img(j).GetPixel(x, y).R Then
                            distance(j) += 1
                        End If
                    Next
                Next
            Next
            For k = 0 To 9
                If distance.Min = distance(k) Then
                    license_number(i) = k
                End If
            Next
        Next

        TextBox1.Text = license_number(0)
        TextBox2.Text = license_number(1)
        TextBox3.Text = license_number(2)
        TextBox4.Text = license_number(3)
        TextBox5.Text = license_number(4)
        TextBox6.Text = license_number(5)
        TextBox7.Text = license_number(6)
    End Sub
End Class
