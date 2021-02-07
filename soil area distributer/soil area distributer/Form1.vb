Imports System.Text.RegularExpressions
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices

Public Class Form1
    Dim h As Integer = 80
    ' insert height in pixels relative to circle radius
    Dim fov As Integer = 50
    Dim colorsstring As List(Of String) = New List(Of String)
    Private _canvas As Bitmap

    Private _wid, _hei As Integer

    Private _g As Graphics

    Private _lastPoint As Point = Nothing

    Private _fillColor As Integer = &HFF0000FF ' AARRGGBB so it's blue

    Private _backgColor As Integer = &HFFFFFFFF ' white

    Private _buffer As ScreenBuffer

    Private _region As New Region

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SerialPort1.Open()
        Timer1.Start()

        Dim whiteback As Bitmap = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        Dim rg As Graphics = Graphics.FromImage(whiteback)
        rg.FillRectangle(Brushes.White, 0, 0, whiteback.Width, whiteback.Height)
        rg.DrawEllipse(Pens.Black, CInt(whiteback.Width / 2 - 200), CInt(whiteback.Height / 2 - 200), 400, 400)
        rg.FillEllipse(Brushes.Black, CInt(whiteback.Width / 2 - 2), CInt(whiteback.Height / 2 - 2), 4, 4)
        rg.Clear(Color.White)
        PictureBox1.BackgroundImage = whiteback
        For g As Integer = 0 To 255 Step 1
            colorsstring.Add("0," & g & ",255")
        Next
        For b As Integer = 254 To 0 Step -1
            colorsstring.Add("0,255," & b)
        Next
        For r As Integer = 1 To 255 Step 1
            colorsstring.Add(r & ",255,0")
        Next
        For g As Integer = 254 To 0 Step -1
            colorsstring.Add("255," & g & ",0")
        Next
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        Dim matches As MatchCollection = Regex.Matches(SerialPort1.ReadExisting, "\*([0-9]+;[0-9]+;[a-z0-9]+.[a-z0-9]+)\*")
        For Each m As Match In matches
            '  MsgBox("")

            Dim listofreadings() As String = m.Value.Split(";")
            Dim angle1 As Integer = Integer.Parse(listofreadings(1).Replace("*", ""))
            Dim angle2 As Integer = 70 - Integer.Parse(listofreadings(0).Replace("*", ""))
            Label1.Text = angle1 & angle2
            Dim bitm As Bitmap = New Bitmap(PictureBox1.BackgroundImage)
            Dim gp As New GraphicsPath
            Dim points As New List(Of Point)
            If angle2 >= 0 Then
                points.Add(New Point(bitm.Width / 2 + Math.Sin(angle1 * Math.PI / 180) * (h * Math.Tan((angle2 - fov / 2) * Math.PI / 180)), bitm.Height / 2 - Math.Cos(angle1 * Math.PI / 180) * (h * Math.Tan((angle2 - fov / 2) * Math.PI / 180))))
                Dim ss = h / Math.Cos(angle2 * Math.PI / 180) * Math.Tan(fov / 2 * Math.PI / 180)
                Dim ss1 = h * Math.Tan(Math.Abs(angle2) * Math.PI / 180)
                Dim angleofss = Math.Atan(ss / ss1) * 180 / Math.PI
                Dim ss2 = Math.Sqrt(ss ^ 2 + ss1 ^ 2)
                Dim x1 = Math.Sin((angle1 - angleofss) * Math.PI / 180) * ss2
                Dim x2 = Math.Sin((angle1 + angleofss) * Math.PI / 180) * ss2
                Dim y1 = Math.Cos((angle1 - angleofss) * Math.PI / 180) * ss2
                Dim y2 = Math.Cos((angle1 + angleofss) * Math.PI / 180) * ss2
                points.Add(New Point(bitm.Width / 2 + x1, bitm.Height / 2 - y1))
                points.Add(New Point(bitm.Width / 2 + Math.Sin(angle1 * Math.PI / 180) * (h * Math.Tan((angle2 + fov / 2) * Math.PI / 180)), bitm.Height / 2 - Math.Cos(angle1 * Math.PI / 180) * (h * Math.Tan((angle2 + fov / 2) * Math.PI / 180))))
                points.Add(New Point(bitm.Width / 2 + x2, bitm.Height / 2 - y2))
            Else
                points.Add(RotatePoint(New Point(bitm.Width / 2 + Math.Sin(angle1 * Math.PI / 180) * (h * Math.Tan((Math.Abs(angle2) - fov / 2) * Math.PI / 180)), bitm.Height / 2 - Math.Cos(angle1 * Math.PI / 180) * (h * Math.Tan((Math.Abs(angle2) - fov / 2) * Math.PI / 180))), New Point(bitm.Width / 2, bitm.Height / 2), 180))

                Dim ss = h / Math.Cos(Math.Abs(angle2) * Math.PI / 180) * Math.Tan(fov / 2 * Math.PI / 180)
                Dim ss1 = h * Math.Tan(Math.Abs(angle2) * Math.PI / 180)
                Dim angleofss = Math.Atan(ss / ss1) * 180 / Math.PI
                Dim ss2 = Math.Sqrt(ss ^ 2 + ss1 ^ 2)
                Dim x1 = Math.Sin((angle1 - angleofss) * Math.PI / 180) * ss2
                Dim x2 = Math.Sin((angle1 + angleofss) * Math.PI / 180) * ss2
                Dim y1 = Math.Cos((angle1 - angleofss) * Math.PI / 180) * ss2
                Dim y2 = Math.Cos((angle1 + angleofss) * Math.PI / 180) * ss2
                points.Add(RotatePoint(New Point(bitm.Width / 2 + x1, bitm.Height / 2 - y1), New Point(bitm.Width / 2, bitm.Height / 2), 180))
                points.Add(RotatePoint(New Point(bitm.Width / 2 + Math.Sin(angle1 * Math.PI / 180) * (h * Math.Tan((Math.Abs(angle2) + fov / 2) * Math.PI / 180)), bitm.Height / 2 - Math.Cos(angle1 * Math.PI / 180) * (h * Math.Tan((Math.Abs(angle2) + fov / 2) * Math.PI / 180))), New Point(bitm.Width / 2, bitm.Height / 2), 180))
                points.Add(RotatePoint(New Point(bitm.Width / 2 + x2, bitm.Height / 2 - y2), New Point(bitm.Width / 2, bitm.Height / 2), 180))
            End If
            Dim sensorvalue = Decimal.Parse(listofreadings(2).Replace("*", ""))
            If sensorvalue > 1021 Then sensorvalue = 105
            If sensorvalue < 1 Then sensorvalue = 1
            Dim rgbstring As String = colorsstring(Math.Floor((sensorvalue / 105 * 1021) - 1))
            Dim brus As SolidBrush = New SolidBrush(Color.FromArgb(rgbstring.Split(",")(0), rgbstring.Split(",")(1), rgbstring.Split(",")(2)))
            gp.AddPolygon(points.ToArray())
            ' g.FillPath(brus, gp)
            Dim clsRegion2 As New System.Drawing.Region(gp)
            _canvas = New Bitmap(bitm.Width, bitm.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
            _g = Graphics.FromImage(_canvas)
            _g.DrawImage(bitm, 0, 0)
            Me.DoubleBuffered = True
            FloodFill(0, 0, clsRegion2)
            '   PictureBox1.BackgroundImage = bitm
        Next
    End Sub

    Private Shared Function RotatePoint(ByVal pointToRotate As Point, ByVal centerPoint As Point, ByVal angleInDegrees As Double) As Point
        Dim angleInRadians As Double = angleInDegrees * (Math.PI / 180)
        Dim cosTheta As Double = Math.Cos(angleInRadians)
        Dim sinTheta As Double = Math.Sin(angleInRadians)
        Return New Point With {
            .X = CInt((cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X)),
            .Y = CInt((sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y))
        }
    End Function

    Private Function CropBitmap(ByRef bmp As Bitmap, ByVal cropX As Integer, ByVal cropY As Integer, ByVal cropWidth As Integer, ByVal cropHeight As Integer) As Bitmap
        Dim rect As New Rectangle(cropX, cropY, cropWidth, cropHeight)
        Dim cropped As Bitmap = bmp.Clone(rect, bmp.PixelFormat)
        Return cropped
    End Function

    Private Sub FloodFill(ByVal startX As Integer, ByVal startY As Integer, ByVal grapath As Region)

        ' getpixel/setpixel are waaaaaaaaaay too slow.

        ' here I copy the pixel data into an int array instead.

        ' the int array is in a custom class with getpixel/setpixel equivalents.

        Dim bmd As Imaging.BitmapData

        ' _canvas rectangle is the same as me.clientrectangle

        _wid = _canvas.Width

        _hei = _canvas.Height

        Dim pixelCount As Integer = (_wid * _hei) - 1

        bmd = _canvas.LockBits(New Rectangle(0, 0, _wid, _hei), Imaging.ImageLockMode.ReadWrite, Imaging.PixelFormat.Format32bppArgb)

        _buffer = New ScreenBuffer(pixelCount, _wid)

        Marshal.Copy(bmd.Scan0, _buffer.PixelArray, 0, _buffer.PixelArray.Length)

        _region.MakeEmpty()

        Fill(startX, startY)

        ' copy the array back

        Marshal.Copy(_buffer.PixelArray, 0, bmd.Scan0, _buffer.PixelArray.Length)

        ' unlock the bitmap so that we can use it again

        _canvas.UnlockBits(bmd)

        ' area is filled in blue already, go over with yellow to show the region works...


        ' create a new form shaped with the region
        Dim rr As New Region
        rr = _region
        rr.Intersect(grapath)
        grapath.Union(_region)
        grapath.Xor(rr)
        Label2.Text = (grapath.IsEmpty(_g))
        _g.FillRegion(Brushes.Yellow, grapath)
        Me.Invalidate(_region)
        PictureBox1.BackgroundImage = _canvas
    End Sub

    Private Sub Fill(ByVal x As Integer, ByVal y As Integer)

        If _buffer.GetPixel(x, y) <> _backgColor Then Exit Sub

        Dim Q As New Queue(Of Point)

        Q.Enqueue(New Point(x, y))

        Dim w, e As Integer

        Dim p As Point

        Dim done As Boolean = False

        Dim gp As New Drawing2D.GraphicsPath

        Do

            done = False

            ' get a starting point that hasn't already been set to fillcolor

            Do

                p = Q.Dequeue

                If _buffer.GetPixel(p.X, p.Y) = _backgColor Then done = True

            Loop Until done Or Q.Count = 0

            ' if there isn't a point that isn;t set then quit loop

            If Q.Count = 0 And _buffer.GetPixel(p.X, p.Y) <> _backgColor Then Exit Do

            ' now scan west, then east to find the first pixel that is not the background color

            done = False

            w = p.X

            Do

                If w = 0 Then done = True

                If Not done AndAlso _buffer.GetPixel(w, p.Y) <> _backgColor Then done = True

                If Not done Then w -= 1

            Loop Until done

            e = p.X

            done = False

            Do

                If e = _wid Then done = True

                If Not done AndAlso _buffer.GetPixel(e, p.Y) <> _backgColor Then done = True

                If Not done Then e += 1

            Loop Until done

            Dim n, s As Integer

            n = p.Y

            ' moving up, scan each row looking for a non background color

            done = False

            Do

                If n = 0 Then done = True

                If Not done Then

                    For i As Integer = w + 1 To e - 1

                        If _buffer.GetPixel(i, n) <> _backgColor Then

                            done = True ' northern limit of rectangle

                        End If

                    Next

                End If

                If Not done Then n -= 1

            Loop Until done

            s = p.Y

            ' moving down, scan each row looking for a non background color

            done = False

            Do

                If s = _hei Then done = True

                If Not done Then

                    For i As Integer = w + 1 To e - 1

                        If _buffer.GetPixel(i, s) <> _backgColor Then

                            done = True ' southern limit of rectangle

                        End If

                    Next

                End If

                If Not done Then s += 1

            Loop Until done

            ' fill the found rectangle (required as otherwise the adjacent rectangles will find it again as whitespace)

            ' so you cant rely on the fillregion

            For x1 As Integer = w + 1 To e - 1

                For y1 As Integer = n + 1 To s - 1

                    _buffer.SetPixel(x1, y1, _fillColor)

                Next

            Next

            Dim lastNorthWasQueued As Boolean = False

            Dim lastSouthWasQueued As Boolean = False

            ' queue valid points along top and bottom of rec

            ' say we are moving along the top row, we find a white pixel and mark it as a

            ' starting point for a new check. We move to the right and find another.

            ' there is no point checking from this point as it will be found in the search

            ' started at the neighbouring point. So skip adjacent white points.

            For i As Integer = w + 1 To e - 1

                If n > 0 Then

                    If _buffer.GetPixel(i, n) = _backgColor And lastNorthWasQueued = False Then

                        Q.Enqueue(New Point(i, n))

                        lastNorthWasQueued = True

                    Else

                        lastNorthWasQueued = False

                    End If

                End If

                If s < _hei - 1 Then

                    If _buffer.GetPixel(i, s) = _backgColor And lastSouthWasQueued = False Then

                        Q.Enqueue(New Point(i, s))

                        lastSouthWasQueued = True

                    Else

                        lastSouthWasQueued = False

                    End If

                End If

            Next

            ' queue valid points along the left and right edges of rec

            Dim lastEastWasQueued As Boolean = False

            Dim lastWestWasQueued As Boolean = False

            For i As Integer = n + 1 To s - 1

                If w > 0 Then

                    If _buffer.GetPixel(w, i) = _backgColor And lastWestWasQueued = False Then

                        Q.Enqueue(New Point(w, i))

                        lastWestWasQueued = True

                    Else

                        lastWestWasQueued = False

                    End If

                End If

                If e < _wid - 1 Then

                    If _buffer.GetPixel(e, i) = _backgColor And lastEastWasQueued = False Then

                        Q.Enqueue(New Point(e, i))

                        lastEastWasQueued = True

                    Else

                        lastEastWasQueued = False

                    End If

                End If

            Next

            ' Add to the region:

            _region.Union(New Rectangle(w + 1, n + 1, e - w - 1, s - n - 1))

        Loop Until Q.Count = 0

    End Sub
End Class

Public Class ScreenBuffer

    Private _pixels() As Integer

    Private _width As Integer

    Public Sub New(ByVal pixelCount As Integer, ByVal wid As Integer)

        ReDim _pixels(pixelCount)

        _width = wid

    End Sub

    Public Property PixelArray() As Integer()

        Get

            Return _pixels

        End Get

        Set(ByVal value As Integer())

            _pixels = value

        End Set

    End Property



    Public Function GetPixel(ByVal x As Integer, ByVal y As Integer) As Integer

        Return _pixels(Position(x, y))

    End Function



    Public Sub SetPixel(ByVal x As Integer, ByVal y As Integer, ByVal col As Integer)

        _pixels(Position(x, y)) = col

    End Sub



    Private Function Position(ByVal x As Integer, ByVal y As Integer) As Integer

        ' get position in array from x and y values

        Return x + (y * _width)

    End Function

End Class