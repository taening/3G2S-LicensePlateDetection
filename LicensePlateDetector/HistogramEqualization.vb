Public Class HistogramEqualization
    Dim intensity_array(255) As Integer
    Dim normalizedSum_array(255) As Integer

    Public Function ipHistogramEqualization(image As Bitmap) As Bitmap
        Dim heImage As New Bitmap(image)
        Dim total_pixel As Integer = heImage.Width * heImage.Height
        Dim max_intensity As Integer = 255
        Dim sum, gray As Integer

        '히스토그램 구성
        For y = 0 To heImage.Height - 1
            For x = 0 To heImage.Width - 1
                intensity_array(heImage.GetPixel(x, y).R) += 1
            Next
        Next

        'Normalized Sum 구하기
        For i = 0 To intensity_array.Length - 1
            sum += intensity_array(i)
            normalizedSum_array(i) = sum * (max_intensity / total_pixel)
        Next

        'Normalized Sum에 따라 Intensity 조정
        For y = 0 To heImage.Height - 1
            For x = 0 To heImage.Width - 1
                gray = heImage.GetPixel(x, y).R
                heImage.SetPixel(x, y, Color.FromArgb(normalizedSum_array(gray), normalizedSum_array(gray), normalizedSum_array(gray)))
            Next
        Next

        Return heImage
    End Function
End Class
