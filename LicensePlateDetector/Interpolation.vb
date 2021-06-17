Public Class Interpolation
    '보간법을 실행하는 함수
    Public Function ipInterpolation(image As Bitmap, width As Integer, height As Integer, cubic As Double) As Bitmap
        Dim ipImage As New Bitmap(image)
        Dim image_x As New Bitmap(width, image.Height)
        Dim image_y As New Bitmap(width, height)
        Dim red, green, blue As Integer
        Dim xi, yi, result As Double

        For y = 0 To ipImage.Height - 1
            For x = 0 To width - 1
                xi = CType(x, Double) / width * ipImage.Width
                red = 0 : green = 0 : blue = 0
                For kx = 0 To ipImage.Width - 1
                    If (xi - 2.0) <= kx And (xi + 2.0) > kx Then
                        result = getCubic(xi, kx, cubic)

                        red += ipImage.GetPixel(kx, y).R * result
                        green += ipImage.GetPixel(kx, y).G * result
                        blue += ipImage.GetPixel(kx, y).B * result
                    End If
                Next

                If red < 0 Then
                    red = 0
                End If
                If green < 0 Then
                    green = 0
                End If
                If blue < 0 Then
                    blue = 0
                End If
                If red > 255 Then
                    red = 255
                End If
                If green > 255 Then
                    green = 255
                End If
                If blue > 255 Then
                    blue = 255
                End If
                image_x.SetPixel(x, y, Color.FromArgb(red, green, blue))
            Next
        Next

        For y = 0 To height - 1
            For x = 0 To width - 1
                yi = CType(y, Double) / height * image_x.Height
                red = 0 : green = 0 : blue = 0
                For ky = 0 To image.Height - 1
                    If (yi - 2.0) <= ky And (yi + 2.0) > ky Then
                        result = getCubic(yi, ky, cubic)

                        red += image_x.GetPixel(x, ky).R * result
                        green += image_x.GetPixel(x, ky).G * result
                        blue += image_x.GetPixel(x, ky).B * result
                    End If
                Next

                If red < 0 Then
                    red = 0
                End If
                If green < 0 Then
                    green = 0
                End If
                If blue < 0 Then
                    blue = 0
                End If
                If red > 255 Then
                    red = 255
                End If
                If green > 255 Then
                    green = 255
                End If
                If blue > 255 Then
                    blue = 255
                End If
                image_y.SetPixel(x, y, Color.FromArgb(red, green, blue))
            Next
        Next

        Return image_y
    End Function

    Private Function getCubic(x As Double, kx As Double, a As Double) As Double
        Dim dx, fx As Double
        If x > kx Then
            dx = x - kx
        Else
            dx = kx - x
        End If

        If dx < 1 Then
            fx = (a + 2) * dx ^ 3 - (a + 3) * dx ^ 2 + 1
        ElseIf dx < 2 Then
            fx = a * dx ^ 3 - 5 * a * dx ^ 2 + 8 * a * dx - 4 * a
        Else
            fx = 0
        End If

        Return fx
    End Function


End Class
