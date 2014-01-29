using Com.SharpZebra.Commands;
using Com.SharpZebra.Commands.Codes;
using Com.SharpZebra;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace Kaban.KabanAC1040Library.KabanControllers
{
    class LabelBuilder
    {
        private delegate IZebraCommand Command(string value, int x, int y);
        private string value;
        private Command commandToExecute;

        private ZebraCommands commands = new ZebraCommands();

		public LabelBuilder BarcodeHeight(string barcode, int x, int y, int height)
		{
			commands.Add(ZebraCommands.BarCodeCommand(x, y, ElementRotation.NO_ROTATION, 1, 2, 4, height, false, barcode));
			//commands.Add(ZebraCommands.BarCodeCommand(x, y, ElementRotation.NO_ROTATION, 1,  .DrawBox(startX, startY, 2, endX, endY));
			return this;
		}

		public LabelBuilder TextRotated(string text, StandardZebraFont fontsize, int x, int y, ElementRotation rotation)
		{
			commands.Add(ZebraCommands.TextCommand(x, y, rotation, fontsize, 1, 1, false, text));
			//commands.Add(ZebraCommands.BarCodeCommand(x, y, ElementRotation.NO_ROTATION, 1,  .DrawBox(startX, startY, 2, endX, endY));
			return this;
		}

        public LabelBuilder Barcode(string value)
        {
            return StoreValues(BarcodeCommand, value);
        }

        private LabelBuilder StoreValues(Command delegateCommand, string value)
        {
            this.value = value;
            commandToExecute = delegateCommand;
            return this;
        }

        public LabelBuilder At(int x, int y)
        {
            commands.Add(commandToExecute(value, x, y));
            commandToExecute = null;
            value = null;
            return this;
        }

        public LabelBuilder Text(string value)
        {
            return StoreValues(StandardText, value);
        }

        public LabelBuilder Image(string value)
        {
            return StoreValues(ImageCommand, value);
        }

        private IZebraCommand ImageCommand(string imageName, int x, int y)
        {
            return new GraphicZebraCommand(imageName, x, y);
        }

		public LabelBuilder ImageFromStream(System.IO.Stream value, int x, int y)
		{
			commands.Add(new GraphicZebraCommand(value, x, y));
			return this;
		}

		public class ImageZebra : IZebraCommand
		{
			private readonly string deleteStoreAndPrintCommand;

			public ImageZebra(Bitmap _bmp, int x, int y)
			{
				BitmapData bits = _bmp.LockBits(new Rectangle(0, 0, _bmp.Width, _bmp.Height),
				ImageLockMode.ReadOnly, _bmp.PixelFormat);

				byte[] imageBytes = new byte[bits.Height * bits.Stride];
				Marshal.Copy(bits.Scan0, imageBytes, 0, bits.Stride * bits.Height);

				int imgWidth = bits.Stride;
				int imgHeight = bits.Height;
				_bmp.UnlockBits(bits);

				deleteStoreAndPrintCommand = string.Format("GW{0},{1},{2},{3},{4}\n",
					  x,
					  y,
					  ((int)(imgWidth / 8)).ToString(),
					  imgHeight.ToString(),
					  Encoding.GetEncoding(1252).GetString(imageBytes));
			}

			public override string ToString()
			{
				return deleteStoreAndPrintCommand;
			}

			public string ToZebraInstruction()
			{
				return ToString();
			}
		}

		public LabelBuilder Image(Bitmap _bmp, int x, int y)
		{
			commands.Add(new ImageZebra(_bmp, x, y));
			return this;
		}

		public class CodepageZebra : IZebraCommand
		{
			private readonly string deleteStoreAndPrintCommand;

			public CodepageZebra(string value)
			{

				deleteStoreAndPrintCommand = string.Format("I{0},{1},{2}\n",
					  8,
					  "C",
					  "001");
			}

			public override string ToString()
			{
				return deleteStoreAndPrintCommand;
			}

			public string ToZebraInstruction()
			{
				return ToString();
			}
		}

		public LabelBuilder Codepage(string value)
		{
			commands.Add(new CodepageZebra(value));
			return this;
		}

		public class PrintDirectionCommand : IZebraCommand
		{
			public enum Direction: int { FromTop = 'T', FromBottom = 'B' }
			private readonly string deleteStoreAndPrintCommand;

			public PrintDirectionCommand(Direction direction)
			{
				deleteStoreAndPrintCommand = string.Format("Z{0}\n",
					  (char)direction);
			}

			public override string ToString()
			{
				return deleteStoreAndPrintCommand;
			}

			public string ToZebraInstruction()
			{
				return ToString();
			}
		}

		public LabelBuilder PrintDirection(PrintDirectionCommand.Direction direction)
		{
			commands.Add(new PrintDirectionCommand(direction));
			return this;
		}

        private IZebraCommand BarcodeCommand(string barcode, int x, int y)
        {
			return ZebraCommands.BarCodeCommand(x, y, ElementRotation.NO_ROTATION, 1, 2, 4, 50, true, barcode);
        }

        private IZebraCommand StandardText(string text, int x, int y)
        {
            return ZebraCommands.TextCommand(x, y, ElementRotation.NO_ROTATION, StandardZebraFont.NORMAL, 1, 1, false, text);
        }

		private IZebraCommand RotatedText(string text, int x, int y)
		{
			return ZebraCommands.TextCommand(x, y, ElementRotation.NO_ROTATION, StandardZebraFont.NORMAL, 1, 1, false, text);
		}


        public string ToZebraInstruction()
        {
            return commands.ToZebraInstruction();
        }

        public LabelBuilder Border(int startX, int startY, int endX, int endY)
        {
            commands.Add(ZebraCommands.DrawBox(startX, startY, 2, endX, endY));
            return this;
        }

		public LabelBuilder BlackLine(int horizontalStartPositionInDots, int verticalStartPositionInDots, int horizontalLengthInDots, int verticalLengthInDots)
		{
			commands.Add(ZebraCommands.BlackLine(horizontalStartPositionInDots, verticalStartPositionInDots, horizontalLengthInDots, verticalLengthInDots));
			return this;
		}

		public LabelBuilder DiagonalLine(int horizontalStartPositionInDots, int verticalStartPositionInDots, int lineThicknessInDots, int horizontalEndPositionInDots, int verticalEndPositionInDots)
		{
			commands.Add(ZebraCommands.DiagonalLine(horizontalStartPositionInDots, verticalStartPositionInDots, lineThicknessInDots, horizontalEndPositionInDots, verticalEndPositionInDots));
			return this;
		}
    }
}
