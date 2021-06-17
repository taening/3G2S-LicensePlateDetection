Public Class Morphology
    Dim mask(2, 2) As Integer
    '모폴로지 침식연산
    Public Function ipErosion(image As Bitmap) As Bitmap
        Dim erosionImage As New Bitmap(image)
        Dim z(image.Width - 1, image.Height - 1) As Integer
        Dim count As Integer

        For y = 0 To erosionImage.Height - 1
            For x = 0 To erosionImage.Width - 1
                If x > erosionImage.Width - 3 Or y > erosionImage.Height - 3 Then
                    Continue For
                End If

                For ky = 0 To 2
                    For kx = 0 To 2
                        mask(kx, ky) = erosionImage.GetPixel(x + kx, y + ky).R
                        If mask(kx, ky) = 255 Then
                            count += 1
                        End If
                    Next
                Next

                If count = 9 Then
                    z(x + 1, y + 1) = 1
                End If
                count = 0
            Next
        Next

        For y = 0 To erosionImage.Height - 1
            For x = 0 To erosionImage.Width - 1
                If z(x, y) = 1 Then
                    erosionImage.SetPixel(x, y, Color.FromArgb(255, 255, 255))
                Else
                    erosionImage.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                End If
            Next
        Next

        Return erosionImage
    End Function

    '모폴로지 팽창연산
    Public Function ipDilation(image As Bitmap) As Bitmap
        Dim dilationImage As New Bitmap(image)
        Dim k(image.Width - 1, image.Height - 1) As Integer
        Dim New_k(image.Width - 1, image.Height - 1) As Integer

        For y = 0 To dilationImage.Height - 1
            For x = 0 To dilationImage.Width - 1
                '이미지 그레이스케일값 k 배열에 저장
                k(x, y) = dilationImage.GetPixel(x, y).R
            Next
        Next

        For y = 0 To dilationImage.Height - 1
            For x = 0 To dilationImage.Width - 1
                '범위 제한
                If (x < (dilationImage.Width - 3)) And ((x - 1) >= 0) And (y < (dilationImage.Height - 3)) And ((y - 1) >= 0) Then
                    '기준 픽셀의 주변 8방향의 픽셀값을 확인하고 흰 색일 경우 새로운 배열에 흰 색, 검은 색일 경우 기존값(검은 색) 대입
                    If (k(x - 1, y - 1) = 255) Or (k(x, y - 1) = 255) Or (k(x + 1, y - 1) = 255) Or (k(x + 1, y) = 255) Or
                    (k(x + 1, y + 1) = 255) Or (k(x, y + 1) = 255) Or (k(x - 1, y + 1) = 255) Or (k(x - 1, y) = 255) Then
                        New_k(x, y) = 255
                    Else
                        New_k(x, y) = k(x, y)
                    End If
                End If
            Next
        Next

        '새로 구한 배열 이미지에 대입
        For y = 0 To dilationImage.Height - 1
            For x = 0 To dilationImage.Width - 1
                If New_k(x, y) = 255 Then
                    dilationImage.SetPixel(x, y, Color.FromArgb(255, 255, 255))
                Else
                    dilationImage.SetPixel(x, y, Color.FromArgb(0, 0, 0))
                End If
            Next
        Next

        Return dilationImage
    End Function

    '모폴로지 열린연산
    Public Function ipOpening(image As Bitmap) As Bitmap
        Dim openImage As New Bitmap(image)
        openImage = ipErosion(openImage)
        openImage = ipDilation(openImage)

        Return openImage
    End Function

    '모폴로지 닫힌연산
    Public Function ipClosing(image As Bitmap) As Bitmap
        Dim closeImage As New Bitmap(image)
        closeImage = ipDilation(closeImage)
        closeImage = ipErosion(closeImage)

        Return closeImage
    End Function
End Class
