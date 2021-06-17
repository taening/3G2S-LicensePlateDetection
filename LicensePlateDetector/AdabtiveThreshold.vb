Public Class AdabtiveThreshold
    Public Function ipAdaptiveThreshold(image As Bitmap, maxValue As Double, blockSize As Integer, c As Double)
        Dim atImage As New Bitmap(image)
        Dim mask(blockSize - 1, blockSize - 1) As Integer
        Dim test As Integer = blockSize / 2 - 0.5
        Dim mean As Integer

        For y = 0 To atImage.Height - 1
            For x = 0 To atImage.Width - 1
                If x - test < 0 Or y - test < 0 Or x + test > atImage.Width - 1 Or y + test > atImage.Height - 1 Then
                    atImage.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                    Continue For
                End If

                For ky = -test To test
                    For kx = -test To test
                        mask(kx + test, ky + test) = image.GetPixel(x + kx, y + ky).R
                        mean += mask(kx + test, ky + test)
                    Next
                Next
                mean /= blockSize ^ 2
                mean -= c

                If maxValue - mean > 0 Then
                    atImage.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                Else
                    atImage.SetPixel(x, y, Color.FromArgb(255, 255, 255))
                End If
                mean = 0
            Next
        Next

        Return atImage
    End Function
End Class
