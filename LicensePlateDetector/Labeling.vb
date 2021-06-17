Public Class Labeling
    Dim label_list As New ArrayList
    Dim labelNumber As Integer = 0

    '라벨링을 실행하는 함수
    'stack = 후입선출 관련 자료형(관심있으면 자료구조 찾아보시면 좋습니다.)
    'label_list = 서로 연결된 좌표의 정보(에지요소)를 담은 리스트 [edge_array, edge_array, edge_array, ...]
    'label_list.Add(New Integer() {0, 0, 0, 0, 0, 0, 0}) = [0] : 좌상단x, [1] : 좌상단y, [2] : 우하단x, [3] : 우하단y, [4] : 흰색픽셀수, [5] : 검은픽셀수
    Public Function ipLabeling(image As Bitmap, mode As Char) As Bitmap
        Dim lbImage As New Bitmap(image)
        Dim stack As New Stack
        Dim labelImage(image.Width - 1, image.Height - 1) As Integer
        Dim clr As Integer

        If mode = "W" Then
            clr = 255
        ElseIf mode = "B" Then
            clr = 0
        End If

        For x = 0 To lbImage.Width - 1
            For y = 0 To lbImage.Height - 1
                If lbImage.GetPixel(x, y).R <> clr Or labelImage(x, y) <> 0 Then
                    Continue For
                End If
                labelNumber += 1

                stack.Push(New Integer() {x, y})

                Dim kx As Integer
                Dim ky As Integer
                While stack.Count <> 0
                    kx = stack.Peek(0)
                    ky = stack.Peek(1)
                    stack.Pop()

                    labelImage(kx, ky) = labelNumber

                    For x2 = kx - 1 To kx + 1
                        If x2 < 0 Or x2 > lbImage.Width - 1 Then
                            Continue For
                        End If

                        For y2 = ky - 1 To ky + 1
                            If y2 < 0 Or y2 > lbImage.Height - 1 Then
                                Continue For
                            End If

                            If lbImage.GetPixel(x2, y2).R <> clr Or labelImage(x2, y2) <> 0 Then
                                Continue For
                            End If
                            stack.Push(New Integer() {x2, y2})
                            labelImage(x2, y2) = labelNumber
                        Next
                    Next
                End While
            Next
        Next

        For i = 0 To labelNumber - 1
            label_list.Add(New Integer() {0, 0, 0, 0, 0, 0, 0})
        Next

        For x = 0 To lbImage.Width - 1
            For y = 0 To lbImage.Height - 1
                If labelImage(x, y) = 0 Then
                    Continue For
                End If

                '만약 label_list[labelImage(i, j) - 1]의 index <> 1 이면
                'index = 1로 바꾸고 i, j를 label_list[labelImage(i, j)]의 0번과 1번에 저장 (좌상단 좌표)
                If label_list(labelImage(x, y) - 1)(6) <> 1 Then
                    label_list(labelImage(x, y) - 1)(6) = 1
                    label_list(labelImage(x, y) - 1)(0) = x
                    label_list(labelImage(x, y) - 1)(1) = y
                    label_list(labelImage(x, y) - 1)(2) = x
                    label_list(labelImage(x, y) - 1)(3) = y
                Else
                    If label_list(labelImage(x, y) - 1)(0) > x Then
                        label_list(labelImage(x, y) - 1)(0) = x
                    End If
                    If label_list(labelImage(x, y) - 1)(1) > y Then
                        label_list(labelImage(x, y) - 1)(1) = y
                    End If
                    If label_list(labelImage(x, y) - 1)(2) < x Then
                        label_list(labelImage(x, y) - 1)(2) = x
                    End If
                    If label_list(labelImage(x, y) - 1)(3) < y Then
                        label_list(labelImage(x, y) - 1)(3) = y
                    End If
                End If
            Next
        Next

        For i = 0 To label_list.Count - 1
            For y = label_list(i)(1) To label_list(i)(3)
                For x = label_list(i)(0) To label_list(i)(2)
                    If lbImage.GetPixel(x, y).R = 255 Then
                        label_list(i)(4) += 1
                    Else
                        label_list(i)(5) += 1
                    End If
                Next
            Next
        Next

        ' <<Test용 코드>>
        Dim g As Graphics = Graphics.FromImage(lbImage)
        Dim redPen As New Pen(Color.Red, 1)
        Dim aFont As New System.Drawing.Font("Arial", 7, FontStyle.Bold)
        For i = 0 To labelNumber - 1
            Dim rect As New Rectangle(label_list(i)(0), label_list(i)(1), label_list(i)(2) - label_list(i)(0), label_list(i)(3) - label_list(i)(1))
            g.DrawRectangle(redPen, rect)
            'g.DrawString(Str(i + 1), aFont, Brushes.Blue, (label_list(i)(0) + label_list(i)(2)) / 2, (label_list(i)(1) + label_list(i)(3)) / 2)
        Next

        Return lbImage
    End Function

    Public Function getLabelList() As ArrayList
        Return label_list
    End Function
End Class

