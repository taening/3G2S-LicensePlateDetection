Imports System.Collections.ArrayList
Public Class ConerDetection
    Public Function ipConerDetection(image As Bitmap, size As Integer, threshold As Integer) As Bitmap
        Dim g As Graphics = Graphics.FromImage(image)
        Dim redPen As New Pen(Color.Red, 1)

        Dim differencial_matrix(image.Width - 1, image.Height - 1, 1) As Integer
        Dim matrix(size - 1, size - 1, 1) As Integer
        Dim C(1, 1) As Integer
        Dim convolution_matrix(2, 2) As Integer
        Dim tmp(1) As Integer
        Dim lamda(1) As Integer

        Dim lamda1_list As New ArrayList
        Dim lamda2_list As New ArrayList

        For y = 0 To image.Height - 1
            For x = 0 To image.Width - 1
                If x > image.Width - 3 Or y > image.Height - 3 Then
                    Continue For
                End If

                For ky = 0 To 2
                    For kx = 0 To 2
                        convolution_matrix(kx, ky) = image.GetPixel(x + kx, y + ky).R
                    Next
                Next
                tmp = Convolution(convolution_matrix)
                differencial_matrix(x, y, 0) = tmp(0)
                differencial_matrix(x, y, 1) = tmp(1)
            Next
        Next

        For y = 0 To image.Height - 1
            For x = 0 To image.Width - 1
                If x > image.Width - size Or y > image.Height - size Then
                    Continue For
                End If

                For ky = 0 To size - 1
                    For kx = 0 To size - 1
                        For i = 0 To 1
                            matrix(kx, ky, i) = differencial_matrix(x + kx, y + ky, i)
                        Next
                    Next
                Next

                For ky = 0 To size - 1
                    For kx = 0 To size - 1
                        C(0, 0) += matrix(kx, ky, 0) ^ 2
                        C(0, 1) += matrix(kx, ky, 0) * matrix(kx, ky, 1)
                        C(1, 0) = C(0, 1)
                        C(1, 1) += matrix(kx, ky, 1) ^ 2
                    Next
                Next

                lamda = EigenValue(C)
                If lamda(1) > threshold Then
                    lamda1_list.Add(New Integer() {x, y, lamda(0)})
                    lamda2_list.Add(New Integer() {x, y, lamda(1)})
                End If
            Next
        Next

        For i = 0 To lamda2_list.Count - 1
            g.DrawRectangle(redPen, lamda2_list.Item(i)(0), lamda2_list.Item(i)(0), 2, 2)
        Next

        Return image
    End Function

    Private Function EigenValue(C As Integer(,)) As Integer()
        Dim tmp(1) As Integer
        Dim lamda1, lamda2 As Integer
        Dim c1, c2, c3, c4 As Double
        c1 = C(0, 0) : c2 = C(0, 1) : c3 = C(1, 0) : c4 = C(1, 1)

        lamda1 = ((c1 + c3) + Math.Sqrt((c1 + c2) ^ 2 - 4 * (c1 * c3 - c2 ^ 2))) / 2
        lamda2 = ((c1 + c3) - Math.Sqrt((c1 + c2) ^ 2 - 4 * (c1 * c3 - c2 ^ 2))) / 2

        tmp(0) = lamda1
        tmp(1) = lamda2

        Return tmp
    End Function

    Private Function Convolution(ori As Integer(,)) As Integer()
        Dim x_mask(2, 2), y_mask(2, 2) As Integer
        Dim jx, jy As Integer
        Dim tmp(1) As Integer

        '1. Sobel 마스크
        'x_mask(0, 0) = -1 : x_mask(0, 1) = 0 : x_mask(0, 2) = 1
        'x_mask(1, 0) = -2 : x_mask(1, 1) = 0 : x_mask(1, 2) = 2
        'x_mask(2, 0) = -1 : x_mask(2, 1) = 0 : x_mask(2, 2) = 1

        'y_mask(0, 0) = 1 : y_mask(0, 1) = 2 : y_mask(0, 2) = 1
        'y_mask(1, 0) = 0 : y_mask(1, 1) = 0 : y_mask(1, 2) = 0
        'y_mask(2, 0) = -1 : y_mask(2, 1) = -2 : y_mask(2, 2) = -1

        '2. 미분 마스크
        x_mask(0, 0) = 0 : x_mask(0, 1) = 0 : x_mask(0, 2) = 0
        x_mask(1, 0) = -1 : x_mask(1, 1) = 0 : x_mask(1, 2) = 1
        x_mask(2, 0) = 0 : x_mask(2, 1) = 0 : x_mask(2, 2) = 0

        y_mask(0, 0) = 0 : y_mask(0, 1) = -1 : y_mask(0, 2) = 0
        y_mask(1, 0) = 0 : y_mask(1, 1) = 0 : y_mask(1, 2) = 0
        y_mask(2, 0) = 0 : y_mask(2, 1) = 1 : y_mask(2, 2) = 0

        'Convolution과정
        For y = 0 To 2
            For x = 0 To 2
                jx += ori(x, y) * x_mask(x, y)
                jy += ori(x, y) * y_mask(x, y)
            Next
        Next

        If jx < 0 Then
            jx = 0
        End If
        If jy < 0 Then
            jy = 0
        End If

        tmp(0) = jx
        tmp(1) = jy

        Return tmp
    End Function
End Class
