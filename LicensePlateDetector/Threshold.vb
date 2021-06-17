Public Class Threshold
    '이진화를 실행하는 함수
    Public Function ipThresholding(image As Bitmap) As Bitmap
        Dim thImage As New Bitmap(image)
        Dim total_pixel As Integer = thImage.Width * thImage.Height
        Dim dark_ratio, dark_mean As Double
        Dim bright_ratio, bright_mean As Double
        Dim variance, tmp As Double

        For threshold = 80 To 150
            For y = 0 To thImage.Height - 1
                For x = 0 To thImage.Width - 1
                    If thImage.GetPixel(x, y).R <= threshold Then
                        dark_ratio += 1
                        dark_mean += thImage.GetPixel(x, y).R

                    Else
                        bright_ratio += 1
                        bright_mean += thImage.GetPixel(x, y).R
                    End If
                Next
            Next
            dark_mean /= dark_ratio
            bright_mean /= bright_ratio
            dark_ratio /= total_pixel
            bright_ratio /= total_pixel
            variance = dark_ratio * bright_ratio * (dark_mean - bright_mean) ^ 2


            If variance >= tmp Then
                tmp = variance
            Else
                tmp = threshold - 2
                Exit For
            End If

            '변수 초기화
            dark_mean = 0
            bright_mean = 0
            dark_ratio = 0
            bright_ratio = 0
            variance = 0
        Next

        For y = 0 To thImage.Height - 1
            For x = 0 To thImage.Width - 1
                If thImage.GetPixel(x, y).R < tmp Then
                    thImage.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                Else
                    thImage.SetPixel(x, y, Color.FromArgb(255, 255, 255))
                End If
            Next
        Next

        Return thImage
    End Function
End Class
