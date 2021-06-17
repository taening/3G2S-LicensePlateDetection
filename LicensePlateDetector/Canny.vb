Imports System.Collections.ArrayList
Imports System.Collections.Stack

Public Class Canny
    Dim eS(1, 1) As Double
    Dim eO(1, 1) As Double


    Public Function ipCanny(image As Bitmap, thresholdL As Integer, thresholdH As Integer, apertureSize As Integer) As Bitmap
        ReDim eS(image.Width - 1, image.Height - 1)
        ReDim eO(image.Width - 1, image.Height - 1)
        Dim g As New GaussianBlur()

        image = g.ipGaussianBlur(image, apertureSize)
        image = getCannyEnhancer(image)
        image = getNonmaxSuppression(image)
        image = getHystersisThresh(image, thresholdL, thresholdH)

        Return image
    End Function

    Private Function getCannyEnhancer(image As Bitmap) As Bitmap
        Dim matrix(2, 2) As Integer
        Dim tmp(1) As Integer

        For y = 0 To image.Height - 1
            For x = 0 To image.Width - 1
                If x > image.Width - 3 Or y > image.Height - 3 Then
                    Continue For
                End If

                For ky = 0 To 2
                    For kx = 0 To 2
                        matrix(kx, ky) = image.GetPixel(x + kx, y + ky).R
                    Next
                Next
                tmp = Convolution(matrix)
                eS(x + 1, y + 1) = Math.Sqrt(tmp(0) ^ 2 + tmp(1) ^ 2)
                If tmp(0) = 0 Then
                    eO(x + 1, y + 1) = 0
                Else
                    eO(x + 1, y + 1) = (Math.Atan(tmp(1) / tmp(0)) / Math.PI) * 180
                End If
            Next
        Next

        For y = 0 To image.Height - 1
            For x = 0 To image.Width - 1
                If eS(x, y) > 255 Then
                    eS(x, y) = 255
                End If
                image.SetPixel(x, y, Color.FromArgb(eS(x, y), eS(x, y), eS(x, y)))
            Next
        Next

        Return image
    End Function

    Private Function getNonmaxSuppression(image As Bitmap) As Bitmap
        'eO 근사화
        For y = 0 To image.Height - 1
            For x = 0 To image.Width - 1
                If eO(x, y) < 0 Then
                    If eO(x, y) + 180 <= 22.5 Then
                        eO(x, y) = 0
                    ElseIf eO(x, y) + 180 > 22.5 And eO(x, y) + 180 <= 67.5 Then
                        eO(x, y) = 45
                    ElseIf eO(x, y) + 180 > 67.5 And eO(x, y) + 180 <= 112.5 Then
                        eO(x, y) = 90
                    ElseIf eO(x, y) + 180 > 112.5 And eO(x, y) + 180 <= 157.5 Then
                        eO(x, y) = 135
                    Else
                        eO(x, y) = 0
                    End If
                Else
                    If eO(x, y) < 22.5 Then
                        eO(x, y) = 0
                    ElseIf eO(x, y) > 22.5 And eO(x, y) <= 67.5 Then
                        eO(x, y) = 45
                    ElseIf eO(x, y) > 67.5 And eO(x, y) <= 112.5 Then
                        eO(x, y) = 90
                    ElseIf eO(x, y) > 112.5 And eO(x, y) <= 157.5 Then
                        eO(x, y) = 135
                    Else
                        eO(x, y) = 0
                    End If
                End If
            Next
        Next

        For y = 0 To image.Height - 1
            For x = 0 To image.Width - 1
                If x - 1 < 0 Or y - 1 < 0 Or x + 1 > image.Width - 1 Or y + 1 > image.Height - 1 Then
                    Continue For
                End If

                If (eO(x, y) = 0) Then
                    If (eS(x, y + 1) < eS(x, y) And eS(x, y - 1) < eS(x, y)) Then
                        image.SetPixel(x, y, Color.FromArgb(eS(x, y), eS(x, y), eS(x, y)))
                    Else
                        image.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                    End If

                ElseIf (eO(x, y) = 45) Then
                    If (eS(x + 1, y - 1) < eS(x, y) And eS(x - 1, y + 1) < eS(x, y)) Then
                        image.SetPixel(x, y, Color.FromArgb(eS(x, y), eS(x, y), eS(x, y)))
                    Else
                        image.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                    End If

                ElseIf (eO(x, y) = 90) Then
                    If (eS(x + 1, y) < eS(x, y) And eS(x - 1, y) < eS(x, y)) Then
                        image.SetPixel(x, y, Color.FromArgb(eS(x, y), eS(x, y), eS(x, y)))
                    Else
                        image.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                    End If

                ElseIf (eO(x, y) = 135) Then
                    If (eS(x + 1, y + 1) < eS(x, y) And eS(x - 1, y - 1) < eS(x, y)) Then
                        image.SetPixel(x, y, Color.FromArgb(eS(x, y), eS(x, y), eS(x, y)))
                    Else
                        image.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                    End If
                End If
            Next
        Next

        Return image
    End Function

    Private Function getHystersisThresh(image As Bitmap, thresholdL As Integer, thresholdH As Integer) As Bitmap
        Dim stack As New Stack
        Dim recoder(image.Width - 1, image.Height - 1) As Integer
        Dim matrix(2, 2) As Integer

        For y = 0 To image.Height - 1
            For x = 0 To image.Width - 1
                If x > image.Width - 3 Or y > image.Height - 3 Then
                    Continue For
                End If

                If image.GetPixel(x, y).R >= thresholdH Then
                    recoder(x, y) = 255
                ElseIf image.GetPixel(x, y).R < thresholdL Then
                    recoder(x, y) = 0
                Else
                    stack.Push(New Integer() {x, y})

                    Dim kx, ky As Integer
                    While stack.Count <> 0
                        kx = stack.Peek(0) : ky = stack.Peek(1)
                        stack.Pop()

                        If (image.GetPixel(kx, ky).R >= thresholdL And image.GetPixel(kx, ky).R < thresholdH) And recoder(kx, ky) <> 255 Then
                            recoder(kx, ky) = 255
                        End If

                        For y2 = 0 To 2
                            For x2 = 0 To 2
                                If (image.GetPixel(x2, y2).R >= thresholdL And image.GetPixel(x2, y2).R < thresholdH) And recoder(x2, y2) <> 255 Then
                                    stack.Push(New Integer() {x2, y2})
                                End If
                            Next
                        Next
                    End While

                End If
            Next
        Next

        For y = 0 To image.Height - 1
            For x = 0 To image.Width - 1
                If recoder(x, y) = 255 Then
                    image.SetPixel(x, y, Color.FromArgb(255, 255, 255))
                Else
                    image.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                End If
            Next
        Next

        Return image
    End Function

    Private Function getHystersisThresh2(image As Bitmap, thresholdL As Integer, thresholdH As Integer) As Bitmap
        Dim stack As New Stack
        Dim cannyImage(image.Width - 1, image.Height - 1) As Integer
        Dim cannyNumber As Integer = 1
        Dim degree As Integer = 0

        For y = 0 To image.Height - 1
            For x = 0 To image.Width - 1
                If image.GetPixel(x, y).R < thresholdH Or cannyImage(x, y) <> 0 Then
                    Continue For
                End If
                stack.Push(New Integer() {x, y})

                Dim kx As Integer
                Dim ky As Integer
                While stack.Count <> 0
                    kx = stack.Peek(0)
                    ky = stack.Peek(1)
                    stack.Pop()

                    If kx - 1 < 0 Or ky - 1 < 0 Or kx + 1 > image.Width - 1 Or ky + 1 > image.Height - 1 Then
                        Continue For
                    End If
                    cannyImage(kx, ky) = cannyNumber
                    degree = eO(kx, ky) + 90

                    If degree = 90 Then
                        If image.GetPixel(kx - 1, ky).R > thresholdL And cannyImage(kx - 1, ky) = 0 Then
                            stack.Push(New Integer() {kx - 1, ky})
                            cannyImage(kx - 1, ky) = cannyNumber
                        End If

                        If image.GetPixel(kx + 1, ky).R > thresholdL And cannyImage(kx + 1, ky) = 0 Then
                            stack.Push(New Integer() {kx + 1, ky})
                            cannyImage(kx + 1, ky) = cannyNumber
                        End If

                    ElseIf degree = 135 Then
                        If image.GetPixel(kx - 1, ky - 1).R > thresholdL And cannyImage(kx - 1, ky - 1) = 0 Then
                            stack.Push(New Integer() {kx - 1, ky - 1})
                            cannyImage(kx - 1, ky - 1) = cannyNumber
                        End If

                        If image.GetPixel(kx + 1, ky + 1).R > thresholdL And cannyImage(kx + 1, ky + 1) = 0 Then
                            stack.Push(New Integer() {kx + 1, ky + 1})
                            cannyImage(kx + 1, ky + 1) = cannyNumber
                        End If

                    ElseIf degree = 180 Then
                        If image.GetPixel(kx, ky + 1).R > thresholdL And cannyImage(kx, ky + 1) = 0 Then
                            stack.Push(New Integer() {kx, ky + 1})
                            cannyImage(kx, ky + 1) = cannyNumber
                        End If
                        If image.GetPixel(kx, ky - 1).R > thresholdL And cannyImage(kx, ky - 1) = 0 Then
                            stack.Push(New Integer() {kx, ky - 1})
                            cannyImage(kx, ky - 1) = cannyNumber
                        End If
                    ElseIf degree = 225 Then
                        If image.GetPixel(kx + 1, ky - 1).R > thresholdL And cannyImage(kx + 1, ky - 1) = 0 Then
                            stack.Push(New Integer() {kx + 1, ky - 1})
                            cannyImage(kx + 1, ky - 1) = cannyNumber
                        End If
                        If image.GetPixel(kx - 1, ky + 1).R > thresholdL And cannyImage(kx - 1, ky + 1) = 0 Then
                            stack.Push(New Integer() {kx - 1, ky + 1})
                            cannyImage(kx - 1, ky + 1) = cannyNumber
                        End If
                    End If
                End While
            Next
        Next

        Return image
    End Function

    Private Function Convolution(ori As Integer(,)) As Integer()
        Dim x_mask(2, 2), y_mask(2, 2), tmp(1) As Integer
        Dim jx, jy As Integer

        '1. Sobel 마스크
        x_mask(0, 0) = -1 : x_mask(0, 1) = 0 : x_mask(0, 2) = 1
        x_mask(1, 0) = -2 : x_mask(1, 1) = 0 : x_mask(1, 2) = 2
        x_mask(2, 0) = -1 : x_mask(2, 1) = 0 : x_mask(2, 2) = 1

        y_mask(0, 0) = 1 : y_mask(0, 1) = 2 : y_mask(0, 2) = 1
        y_mask(1, 0) = 0 : y_mask(1, 1) = 0 : y_mask(1, 2) = 0
        y_mask(2, 0) = -1 : y_mask(2, 1) = -2 : y_mask(2, 2) = -1

        '2. 미분 마스크
        'x_mask(0, 0) = 0 : x_mask(0, 1) = 0 : x_mask(0, 2) = 0
        'x_mask(1, 0) = -1 : x_mask(1, 1) = 0 : x_mask(1, 2) = 1
        'x_mask(2, 0) = 0 : x_mask(2, 1) = 0 : x_mask(2, 2) = 0

        'y_mask(0, 0) = 0 : y_mask(0, 1) = -1 : y_mask(0, 2) = 0
        'y_mask(1, 0) = 0 : y_mask(1, 1) = 0 : y_mask(1, 2) = 0
        'y_mask(2, 0) = 0 : y_mask(2, 1) = 1 : y_mask(2, 2) = 0

        'Convolution과정
        For y = 0 To 2
            For x = 0 To 2
                jx += ori(x, y) * x_mask(x, y)
                jy += ori(x, y) * y_mask(x, y)
            Next
        Next
        tmp(0) = jx
        tmp(1) = jy

        Return tmp
    End Function
End Class
