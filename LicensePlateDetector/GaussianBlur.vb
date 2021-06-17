Public Class GaussianBlur
    Dim blur_mask(2, 2) As Double
    Dim matrix_size As Integer

    Public Function ipGaussianBlur(image As Bitmap, size As Integer) As Bitmap
        Dim blurImage As New Bitmap(image)
        Dim matrix(size - 1, size - 1) As Integer
        Dim result As Double
        setBlurMask(size)

        For y = 0 To blurImage.Height - 1
            For x = 0 To blurImage.Width - 1
                If x > blurImage.Width - size Or y > blurImage.Height - size Then
                    Continue For
                End If

                For ky = 0 To size - 1
                    For kx = 0 To size - 1
                        matrix(kx, ky) = image.GetPixel(x + kx, y + ky).R
                    Next
                Next
                result = Convolution(matrix, size)

                blurImage.SetPixel(x + (size / 2 - 0.5), y + (size / 2 - 0.5), Color.FromArgb(result, result, result))
            Next
        Next

        Return blurImage
    End Function

    Private Sub setBlurMask(size As Integer)
        Dim sigma As Double
        If size Mod 2 <> 0 And size >= 3 Then
            matrix_size = size
            ReDim blur_mask(matrix_size - 1, matrix_size - 1)
        Else
            End
        End If

        sigma = getSigma((matrix_size / 2) - 0.5, (matrix_size / 2) - 0.5)

        Dim x_max, y_max As Integer
        x_max = (matrix_size / 2) - 0.5
        y_max = (matrix_size / 2) - 0.5

        For x As Integer = 0 To x_max
            For y As Integer = 0 To y_max
                If x = 0 And y = 0 Then
                    blur_mask(x_max, y_max) = getGaussian(sigma, x, y)
                Else
                    blur_mask(x_max + x, y_max + y) = getGaussian(sigma, x, y)
                    blur_mask(x_max + x, y_max - y) = getGaussian(sigma, x, y)
                    blur_mask(x_max - x, y_max + y) = getGaussian(sigma, x, y)
                    blur_mask(x_max - x, y_max - y) = getGaussian(sigma, x, y)
                End If
            Next
        Next

        Dim total As Double
        For x As Integer = 0 To Math.Sqrt(blur_mask.Length) - 1
            For y As Integer = 0 To Math.Sqrt(blur_mask.Length) - 1
                total += blur_mask(x, y)
            Next
        Next

        For x As Integer = 0 To Math.Sqrt(blur_mask.Length) - 1
            For y As Integer = 0 To Math.Sqrt(blur_mask.Length) - 1
                blur_mask(x, y) = blur_mask(x, y) / total
            Next
        Next
    End Sub

    Private Function getSigma(x As Integer, y As Integer) As Double
        Dim sigma As Double
        Dim g As Double

        sigma = matrix_size / 2 - 0.5
        Do
            g = getGaussian(sigma, x, y)
            sigma -= 0.001

        Loop While g > 0.001
        Return sigma
    End Function

    Private Function getGaussian(sigma As Double, x As Integer, y As Integer) As Double
        Dim gaussian As Double
        gaussian = Math.Exp(-(x ^ 2 + y ^ 2) / (2 * sigma ^ 2)) / (2 * Math.PI * sigma ^ 2)

        Return gaussian
    End Function

    Private Function Convolution(ori As Integer(,), size As Integer) As Double
        Dim result As Double

        For y = 0 To size - 1
            For x = 0 To size - 1
                result += ori(x, y) * blur_mask(x, y)
            Next
        Next

        Return result
    End Function
End Class
