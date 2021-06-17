Public Class LicenseSearch
    Public Function ipLicenseSearch(ori As Bitmap, labelW_list As ArrayList, labelB_list As ArrayList) As ArrayList
        Dim result_image As New Bitmap(ori.Width, ori.Height)
        Dim tmp_list As New ArrayList

        tmp_list = NoiseElimination(ori, labelW_list, labelB_list)

        Return tmp_list
    End Function

    Private Function NoiseElimination(ori As Bitmap, labelW_list As ArrayList, labelB_list As ArrayList) As ArrayList
        Dim count As Integer
        Dim label_width, label_height As Integer
        Dim black_label As Integer
        Dim temp_list As ArrayList
        temp_list = labelW_list.Clone()

        '번호판의 잡영제거 조건
        While count <> labelW_list.Count
            label_width = labelW_list(count)(2) - labelW_list(count)(0)
            label_height = labelW_list(count)(3) - labelW_list(count)(1)
            If label_width < label_height Then
                temp_list.Remove(labelW_list(count))
            ElseIf ori.Width * 0.8 <= label_width Then
                temp_list.Remove(labelW_list(count))
            ElseIf ori.Width * 0.8 <= label_width And ori.Height * 0.8 <= label_height Then
                temp_list.Remove(labelW_list(count))
            ElseIf ori.Height * 0.6 <= label_height And ori.Height * 0.6 <= label_height Then
                temp_list.Remove(labelW_list(count))
            ElseIf ori.Width * 0.4 <= label_width And ori.Height * 0.4 <= label_height Then
                temp_list.Remove(labelW_list(count))
            ElseIf ori.Width * 0.05 >= label_width Or ori.Height * 0.05 >= label_height Then
                temp_list.Remove(labelW_list(count))
            ElseIf labelW_list(count)(4) < labelW_list(count)(5) Then
                temp_list.Remove(labelW_list(count))
            ElseIf label_width < 1.5 * label_height Then
                temp_list.Remove(labelW_list(count))
            End If
            count += 1
        End While
        labelW_list = temp_list

        '숫자의 잡영제거 조건
        count = 0
        temp_list = labelB_list.Clone()
        While count <> labelB_list.Count
            label_width = labelB_list(count)(2) - labelB_list(count)(0)
            label_height = labelB_list(count)(3) - labelB_list(count)(1)

            If label_width > label_height Then
                temp_list.Remove(labelB_list(count))
            ElseIf ori.Width * 0.01 >= label_width Or ori.Height * 0.01 >= label_height Then
                temp_list.Remove(labelB_list(count))
            End If
            count += 1
        End While
        labelB_list = temp_list

        count = 0
        temp_list = labelW_list.Clone()
        While count <> labelW_list.Count
            For i = 0 To labelB_list.Count - 1
                If labelW_list(count)(0) < labelB_list(i)(0) And labelW_list(count)(1) < labelB_list(i)(1) And labelW_list(count)(2) > labelB_list(i)(2) And labelW_list(count)(3) > labelB_list(i)(3) Then
                    black_label = black_label + 1
                End If
            Next
            If black_label < 3 Or black_label > 20 Then
                temp_list.Remove(labelW_list(count))
            End If
            black_label = 0
            count += 1
        End While
        labelW_list = temp_list

        Return labelW_list
    End Function
End Class
