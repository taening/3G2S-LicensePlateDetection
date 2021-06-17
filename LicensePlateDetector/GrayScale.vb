Public Class GrayScale
    '회색이미지로 바꿔주는 함수
    Public Function ipGrayScale(image As Bitmap) As Bitmap
        Dim grayImage As New Bitmap(image)
        Dim rgb As Color
        Dim red, green, blue, gray As Integer

        For y = 0 To grayImage.Height - 1
            For x = 0 To grayImage.Width - 1
                rgb = grayImage.GetPixel(x, y)
                red = rgb.R
                green = rgb.G
                blue = rgb.B
                gray = (red + green + blue) / 3
                grayImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray))
            Next
        Next
        Return grayImage
    End Function
End Class
