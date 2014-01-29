using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using Kaban.KabanAC1040Library.Models;
using FastReport.Barcode;
using FastReport;
using FastReport.Utils;
using Com.SharpZebra.Printing;
using Com.SharpZebra;
using Com.SharpZebra.Commands.Codes;
using HiComponents.IEvolution;

namespace Kaban.KabanAC1040Library.KabanControllers
{
	// Tomilov Alexander 17/10/2010
	// Модуль не мой, я его только причесал, чтоб он выглядел не столь уебищно (после просмотра кода представьте насколько был уебищен изначальный код)
	// + прилеплены структуры из дб-контролллера, а то омдуль еще и сам читал и парсил базу
	// + переделана генерация штрихкода на фастрепортовский
	// фаст-репорт не взлетел, ебашим напрямую на принтер
    public class PrintBarcode
    {		
		private PrintDocument printDocument1 = new PrintDocument();
		private PictureBox pictBarcode = new PictureBox();

		private List<PIECE_BARCODE> PieceBarCodes;
		private List<BARCODE_TEMPLATE> BarCodeTemplates;		
		
		private struct ForParce
        {
            public string Name;
            public object Value;
        };

		public PrintBarcode(List<PIECE_BARCODE> pieceBarCodes, List<BARCODE_TEMPLATE> barCodeTemplates)
		{
			PieceBarCodes = pieceBarCodes;
			BarCodeTemplates = barCodeTemplates;
			GetTypesAndValues();
			PrintToZebra();
			#region старая печать
			/*
			printDocument1.PrintController = new StandardPrintController();
            printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument1_PrintPage);
            
			//GetTypesAndValues();

            printDocument1.Print();
			 * /**/
			#endregion старая печать
		}
		
		private List<ForParce> WorkWithVariables(string str)
		{
			List<ForParce> name = new List<ForParce>();
			string res = str;
			string[] find = res.Split('[');
			if (find.Length > 1)
			{
				for (int i = 0; i < find.Length; i++)
				{
					string[] findInfind = find[i].Split(']');
					if (findInfind[0] != "")
					{
						ForParce el = new ForParce();
						el.Name = findInfind[0];
						PIECE_BARCODE pbc = PieceBarCodes.FirstOrDefault(p => p.PIECE_VARIABLE.PIV_NAME == el.Name);
						el.Value = pbc == null ? new byte[] {} : pbc.PIB_VALUE;
						name.Add(el);
					}
				}
			}
			
			return name;
		}

		private void GetTypesAndValues()
        {            
			foreach (var template in BarCodeTemplates)
			{				
				if (template.BARCODE_TYPE.BCT_NAME == "RECT")
					template.Value = null;
				else
					if ( 
						(template.BARCODE_TYPE.BCT_NAME == "IMG") && 
						(Encoding.Default.GetString((byte[])template.BTE_VALUE).Length > 100)
						)
						template.Value = (byte[])template.BTE_VALUE;                    
                    else                 
   						template.Value = Encoding.Default.GetString((byte[])template.BTE_VALUE);

				if ((template.BARCODE_TYPE.BCT_NAME == "LBL") || (template.BARCODE_TYPE.BCT_NAME == "BCD") || (template.BARCODE_TYPE.BCT_NAME == "LINE"))
                {
					List<ForParce> name = WorkWithVariables(template.Value.ToString());
                    for (int j = 0; j < name.Count; j++)                    
                        if (name[j].Value.GetType().Name != "String")                        
							template.Value = template.Value.ToString().Replace("[" + name[j].Name + "]", Encoding.Default.GetString((byte[])name[j].Value));                        
                        else                        
                            if (name[j].Value.ToString() == "")                            
								template.Value = template.Value.ToString().Replace("[" + name[j].Name + "]", "");                                                                      

                }
				if (template.BARCODE_TYPE.BCT_NAME == "IMG")
                {
					if (Encoding.Default.GetString((byte[])template.Value).Length <= 100)
                    {
						List<ForParce> name = WorkWithVariables(template.Value.ToString());
                        for (int j = 0; j < name.Count; j++)                        
							template.Value = (byte[])name[j].Value;                        
                    }
                }
			}           
        }

		private string DefaultPrinterName()
		{
			string functionReturnValue = null;
			var oPS = new PrinterSettings();

			try
			{
				functionReturnValue = oPS.PrinterName;
			}
			catch (System.Exception ex)
			{
				functionReturnValue = "";
			}
			finally
			{
				oPS = null;
			}
			return functionReturnValue;
		}

		private void Print(IZebraCommand command)
		{
			string zebraInstructions = command.ToZebraInstruction();
			var PrinterName = System.Configuration.ConfigurationManager.AppSettings["PrinterName"];
			if (string.IsNullOrEmpty(PrinterName))
				PrinterName = DefaultPrinterName();
			
			new ZebraPrinter(PrinterName).Print(zebraInstructions);
		}

		private void PrintToZebra()
		{
			Print(new BorderedLabel(PieceBarCodes, BarCodeTemplates));
		}

		public class BorderedLabel : IZebraCommand
		{
			private readonly string commandString;

			private Image DownloadImage(byte[] blob)
			{
				MemoryStream ms = new MemoryStream(blob);
				Bitmap bmp = new Bitmap(ms);
				return bmp;
			}

			public BorderedLabel(List<PIECE_BARCODE> PieceBarCodes, List<BARCODE_TEMPLATE> BarCodeTemplates)
			{
				LabelBuilder builder = new LabelBuilder();
				builder =
					builder.Codepage("").
					PrintDirection(LabelBuilder.PrintDirectionCommand.Direction.FromBottom);

				Dictionary<int, string> dct = new Dictionary<int, string>();
				int[] sizes = new int[] { 0, 9, 11, 17, 36 };
				foreach (var i in sizes)
					dct.Add(i, System.Configuration.ConfigurationManager.AppSettings["FontSize" + i.ToString()]);

				var templates =
					from b in BarCodeTemplates
					let ob = b.BARCODE_TYPE.BCT_NAME == "BCD" ? 1 : 0
					orderby ob
					select b;

				foreach (var template in templates)
				//foreach (var template in BarCodeTemplates)
				{
					switch (template.BARCODE_TYPE.BCT_NAME)
					{
						case "IMG":
							{
								// TODO: картинки на печать отправлять
								#region "IMG"
								float x = (float)template.BTE_X1;
								float y = (float)template.BTE_Y1;
								float angle = (float)template.BTE_ANGLE;

								Matrix matrix = new Matrix();
								matrix.RotateAt(angle, new PointF(x, y));
								//g.Transform = matrix;

								//g.DrawImage(DownloadImage((byte[])template.Value), x, y);
								#endregion "IMG"
								break;
							}
						case "LBL":
							{
								#region "LBL"
								float x = (float)template.BTE_X1;
								float y = (float)template.BTE_Y1;
								float angle = (float)template.BTE_ANGLE;
								string fontName = template.BTE_FONTNAME;
								float fontSize = (int)template.BTE_FONTSIZE;
								int halign = (int)template.BTE_HALIGN;
								int valign = (int)template.BTE_VALIGN;
								ElementRotation er;
								switch ((int)angle)
								{
									case 90:
										{
											er = ElementRotation.ROTATE_90_DEGREES;
											break;
										}
									case 180:
										{
											er = ElementRotation.ROTATE_180_DEGREES;
											break;
										}
									case 270:
										{
											er = ElementRotation.ROTATE_270_DEGREES;
											break;
										}
									case 0:
									default:
										{
											er = ElementRotation.NO_ROTATION;
											break;
										}
								}
								StandardZebraFont font;
								/*
								if (fontSize > 30)
									font = StandardZebraFont.LARGEST;
								else
									if (fontSize > 10)
										font = StandardZebraFont.LARGE;
									else
										if (fontSize > 8)
											font = StandardZebraFont.NORMAL;
										else
											if (fontSize > 6)
												font = StandardZebraFont.SMALL;
											else
												font = StandardZebraFont.SMALLEST;

								/**/

								if (fontSize >= 36)
									font = new StandardZebraFont(dct[36]);
								else
									if (fontSize >= 17)
										font = new StandardZebraFont(dct[17]);
										else
											if (fontSize >= 11)
												font = new StandardZebraFont(dct[11]);
											else
												if (fontSize >= 9)
													font = new StandardZebraFont(dct[9]);
												else font = new StandardZebraFont(dct[0]);

								builder = builder.TextRotated(template.Value.ToString(), font, (int)x, (int)y, er);
								#endregion "LBL"
								break;
							}
						case "BCD":
							{
								#region "BCD"
								float x = (float)template.BTE_X1;
								float y = (float)template.BTE_Y1;
								float angle = (float)template.BTE_ANGLE;
								float height = Math.Abs(y - (float)template.BTE_Y2);

								int halign = (int)template.BTE_HALIGN;
								int valign = (int)template.BTE_VALIGN;

								
								double heightAll = height;

								switch (valign)
								{
									case 2:
										{
											y = y - (float)heightAll;
											break;
										}
									case 3:
										{
											y = y - (float)heightAll / 2;
											break;
										}
									case 1:
									default:
										{
											break;
										}
								}

								builder = builder.BarcodeHeight(template.Value.ToString(), (int)x, (int)y, (int)height);

								#endregion "BCD"
								break;
							}
						case "LINE":
							{
								#region "LINE"
								if (!((template.Value.ToString() == "") || (template.Value.ToString() == "0")))
								{
									float x1 = (float)template.BTE_X1;
									float y1 = (float)template.BTE_Y1;
									float x2 = (float)template.BTE_X2;
									float y2 = (float)template.BTE_Y2;

									builder = builder.DiagonalLine((int)x1, (int)y1, 1, (int)x2, (int)y2);
								}
								#endregion "LINE"
								break;
							}
						default:
							break;
					}
				}

				commandString = builder.ToZebraInstruction();
			}

			#region not worked print image
			public void BorderedLabelOld(List<PIECE_BARCODE> PieceBarCodes, List<BARCODE_TEMPLATE> BarCodeTemplates)
			{
				/*
				commandString = new LabelBuilder()
					.Border(5, 5, 200, 200)
					.Barcode(barcode).At(15, 50)
					.Text(line1).At(15, 150)
					.Text(line2).At(15, 250)
					.Image("printer.pcx").At(150, 150).ToZebraInstruction();
				/**/
				LabelBuilder builder = new LabelBuilder();
				using (Bitmap bmp = new Bitmap(800, 180))//, System.Drawing.Imaging.PixelFormat. Format1bppIndexed))
				{
					Graphics g = Graphics.FromImage(bmp);// new BitConverter e.Graphics;
					g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, bmp.Width, bmp.Height));
					var templates =
						from b in BarCodeTemplates
						let ob = b.BARCODE_TYPE.BCT_NAME == "BCD" ? 1 : 0
						orderby ob
						select b;

					foreach (var template in templates)
					{
						switch (template.BARCODE_TYPE.BCT_NAME)
						{
							case "IMG":
								{
									#region "IMG"
									float x = (float)template.BTE_X1;
									float y = (float)template.BTE_Y1;
									float angle = (float)template.BTE_ANGLE;

									Matrix matrix = new Matrix();
									matrix.RotateAt(angle, new PointF(x, y));
									g.Transform = matrix;

									g.DrawImage(DownloadImage((byte[])template.Value), x, y);
									#endregion "IMG"
									break;
								}
							case "LBL":
								{
									#region "LBL"
									float x = (float)template.BTE_X1;
									float y = (float)template.BTE_Y1;
									float angle = (float)template.BTE_ANGLE;
									string fontName = template.BTE_FONTNAME;
									float fontSize = (int)template.BTE_FONTSIZE;
									int halign = (int)template.BTE_HALIGN;
									int valign = (int)template.BTE_VALIGN;

									Font fnt = new Font(fontName, fontSize);
									SizeF size = g.MeasureString(template.Value.ToString(), fnt);
									Matrix matrix = new Matrix();
									matrix.RotateAt(angle, new PointF(x, y));
									g.Transform = matrix;
									PointF drawPoint = new PointF();
									switch (halign)
									{
										case 1:
											{
												drawPoint.X = x;
												break;
											}
										case 2:
											{
												drawPoint.X = x - size.Width;
												break;
											}
										case 3:
											{
												drawPoint.X = x - size.Width / 2;
												break;
											}
										default:
											{
												break;
											}
									}
									switch (valign)
									{
										case 1:
											{
												drawPoint.Y = y;
												break;
											}
										case 2:
											{
												drawPoint.Y = y - size.Height;
												break;
											}
										case 3:
											{
												drawPoint.Y = y - size.Height / 2;
												break;
											}
										default:
											{
												break;
											}
									}

									g.DrawString(template.Value.ToString(), fnt, new SolidBrush(Color.Black), drawPoint);
									#endregion "LBL"
									break;
								}
							case "BCD":
								{
									#region "BCD"
									float x = (float)template.BTE_X1;
									float y = (float)template.BTE_Y1;
									float angle = (float)template.BTE_ANGLE;
									float height = Math.Abs(y - (float)template.BTE_Y2);

									int halign = (int)template.BTE_HALIGN;
									int valign = (int)template.BTE_VALIGN;

									Matrix matrix = new Matrix();
									matrix.RotateAt(angle, new PointF(x, y));
									g.Transform = matrix;
									double heightAll = height;

									switch (valign)
									{
										case 2:
											{
												y = y - (float)heightAll;
												break;
											}
										case 3:
											{
												y = y - (float)heightAll / 2;
												break;
											}
										case 1:
										default:
											{
												break;
											}
									}

									//builder = builder.Image(bmp, (int)x, (int)y);
									builder = builder.Codepage("");
									builder = builder.Text("тест").At(10, 10);
									

									// штрихкод последний в списке, можем рисовать поверх картинки
									using (MemoryStream stream = new MemoryStream())
										using (IEImage ie = new IEImage())
										{
											bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
											stream.Seek(0, SeekOrigin.Begin);
											//builder = builder.ImageFromStream(stream, 0, 0);
											
											stream.Seek(0, SeekOrigin.Begin);

											ie.LoadImage(stream);
											//ie.ConvertToBlackWhite(IEImage.DitherType.Ordered);
											ie.Format = IEImage.PixelFormat.ie1g;// 
											using (MemoryStream ms = new MemoryStream())
											{
												ie.SaveImage(ms, IEFileFormats.PCX);
												bmp.Save(@"d:\atomilov\Projects\Kaban\Output\Debug\KabanAC1040\bmp.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
												using (FileStream mss = new FileStream(@"d:\atomilov\Projects\Kaban\Output\Debug\KabanAC1040\pcx.pcx", FileMode.OpenOrCreate))
												{
													ie.SaveImage(mss, IEFileFormats.PCX);
													ms.Flush();
												}
												ms.Seek(0, SeekOrigin.Begin);
												//builder = builder.Image(@"D:\atomilov\Zebra\sharpzebra-0.90.0.0\src\test\Com.SharpZebra.UnitTests\testImage.pcx").At(200, 15);
												//builder = builder.Image(@"D:\atomilov\Zebra\sharpzebra-0.90.0.0\src\test\Com.SharpZebra.ExampleWinformApplication\printer.pcx").At(0, 0);
												//builder = builder.Image(@"d:\atomilov\Proj
												//builder = builder.ImageFromStream(ms, 0, 0);ects\Kaban\Output\Debug\KabanAC1040\pcx.pcx").At(0, 0);
												
											}
											
										}									

									builder = builder.BarcodeHeight(template.Value.ToString(), (int)x, (int)y, (int)height);

									#endregion "BCD"
									break;
								}
							case "LINE":
								{
									#region "LINE"
									if (!((template.Value.ToString() == "") || (template.Value.ToString() == "0")))
									{
										float x1 = (float)template.BTE_X1;
										float y1 = (float)template.BTE_Y1;
										float x2 = (float)template.BTE_X2;
										float y2 = (float)template.BTE_Y2;

										g.DrawLine(new Pen(Color.Black, 1), x1, y1, x2, y2);
									}
									#endregion "LINE"
									break;
								}
							default:
								break;
						}
					}
				}

				var commandString1 = builder.ToZebraInstruction();
			}
			#endregion not worked print image

			public string ToZebraInstruction()
			{
				return commandString;
			}
		}

		#region старая печать

		private Image DownloadImage(byte[] blob)
		{
			MemoryStream ms = new MemoryStream(blob);
			Bitmap bmp = new Bitmap(ms);
			return bmp;
		}

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			Graphics g = e.Graphics;
			foreach (var template in BarCodeTemplates)
			{
				switch (template.BARCODE_TYPE.BCT_NAME)
				{
					case "IMG":
						{
							#region "IMG"
							float x = (float)template.BTE_X1;
							float y = (float)template.BTE_Y1;
							float angle = (float)template.BTE_ANGLE;
							
							Matrix matrix = new Matrix();
							matrix.RotateAt(angle, new PointF(x, y));
							g.Transform = matrix;
							g.DrawImage(DownloadImage((byte[])template.Value), x, y);
							#endregion "IMG"
							break;
						}
					case "LBL":
						{
							#region "LBL"
							float x = (float)template.BTE_X1;
							float y = (float)template.BTE_Y1;
							float angle = (float)template.BTE_ANGLE;
							string fontName = template.BTE_FONTNAME;
							float fontSize = (int)template.BTE_FONTSIZE;
							int halign = (int)template.BTE_HALIGN;
							int valign = (int)template.BTE_VALIGN;

							Font fnt = new Font(fontName, fontSize);
							SizeF size = g.MeasureString(template.Value.ToString(), fnt);
							Matrix matrix = new Matrix();
							matrix.RotateAt(angle, new PointF(x, y));
							g.Transform = matrix;
							PointF drawPoint = new PointF();
							switch (halign)
							{
								case 1:
									{
										drawPoint.X = x;
										break;
									}
								case 2:
									{
										drawPoint.X = x - size.Width;
										break;
									}
								case 3:
									{
										drawPoint.X = x - size.Width / 2;
										break;
									}
								default:
									{
										break;
									}
							}
							switch (valign)
							{
								case 1:
									{
										drawPoint.Y = y;
										break;
									}
								case 2:
									{
										drawPoint.Y = y - size.Height;
										break;
									}
								case 3:
									{
										drawPoint.Y = y - size.Height / 2;
										break;
									}
								default:
									{
										break;
									}
							}

							g.DrawString(template.Value.ToString(), fnt, new SolidBrush(Color.Black), drawPoint);
							#endregion "LBL"
							break;
						}
					case "BCD":
						{
							#region "BCD"
							float x = (float)template.BTE_X1;
							float y = (float)template.BTE_Y1;
							float angle = (float)template.BTE_ANGLE;							
							float height = Math.Abs(y - (float)template.BTE_Y2);

							int halign = (int)template.BTE_HALIGN;
							int valign = (int)template.BTE_VALIGN;
							
							Matrix matrix = new Matrix();
							matrix.RotateAt(angle, new PointF(x, y));
							g.Transform = matrix;
							double heightAll = height;
							/*
							#region barcode drawings
							BarcodeObject barcode = new BarcodeObject();
							barcode.SymbologyName = "Code128";
							(barcode.Barcode as Barcode128).WideBarRatio = 1;
							barcode.ShowText = false;
							barcode.Text = template.Value.ToString();
							barcode.Height = height - 50;// хер знает почему 50, но только так остатеся расстояние 
							

							// to get actual width, we must draw the barcode first
							using (Bitmap tempBmp = new Bitmap(1, 1))
								using (Graphics g1 = Graphics.FromImage(tempBmp))
									using (GraphicCache cache = new GraphicCache())				
										barcode.Draw(new FRPaintEventArgs(g1, 1, 1, cache));			

							// now we know width and height, perform the drawing
							Bitmap bmpBarCode = new Bitmap((int)barcode.Width + 1, (int)barcode.Height + 1);

							using (Graphics g1 = Graphics.FromImage(bmpBarCode))
								using (GraphicCache cache = new GraphicCache())
									{
										g1.Clear(Color.White);
										barcode.Draw(new FRPaintEventArgs(g1, 1, 1, cache));
									}
							#endregion barcode drawings
							/**/
							int heightShtrih = /*bmpBarCode.Height;// */GenCode128.Code128Rendering.MakeBarcodeImage(template.Value.ToString(), 1, false).Height;
							int CountShtrih = (int)Math.Ceiling(heightAll / heightShtrih);
							PointF drawPoint = new PointF();
							//GenCode128. CodeSet.
							//GenCode128.CodeSet.
							//GenCode128.Code128Rendering.MakeBarcodeImage(template.Value.ToString(), 1, false)
							switch (halign)
							{
								case 1:
									{
										drawPoint.X = x;
										break;
									}
								case 2:
									{
										drawPoint.X = x - /*bmpBarCode.Width;//*/ GenCode128.Code128Rendering.MakeBarcodeImage(template.Value.ToString(), 1, false).Width;
										break;
									}
								case 3:
									{
										drawPoint.X = x - /*bmpBarCode.Width / 2;// */ GenCode128.Code128Rendering.MakeBarcodeImage(template.Value.ToString(), 1, false).Width / 2;
										break;
									}
								default:
									{
										break;
									}
							}

							switch (valign)
							{
								case 1:
									{
										drawPoint.Y = y;
										break;
									}
								case 2:
									{
										drawPoint.Y = y - (float)heightAll;
										break;
									}
								case 3:
									{
										drawPoint.Y = y - (float)heightAll / 2;
										break;
									}
								default:
									{
										break;
									}
							}
						
							for (int j = 0; j < CountShtrih; j++)
							{
								/*
								if (j == CountShtrih - 1)
									g.DrawImage(bmpBarCode, new PointF(drawPoint.X, (float)(drawPoint.Y + heightAll - heightShtrih)));
								else
									g.DrawImage(bmpBarCode, new PointF(drawPoint.X, drawPoint.Y + j * heightShtrih));
								/**/
								//*
								if (j == CountShtrih - 1)								
									g.DrawImage(GenCode128.Code128Rendering.MakeBarcodeImage(template.Value.ToString(), 1, false), new PointF(drawPoint.X, (float)(drawPoint.Y + heightAll - heightShtrih)));								
								else								
									g.DrawImage(GenCode128.Code128Rendering.MakeBarcodeImage(template.Value.ToString(), 1, false), new PointF(drawPoint.X, drawPoint.Y + j * heightShtrih));
								/**/
							}
							#endregion "BCD"
							break;
						}
					case "LINE":
						{
							#region "LINE"
							if (!((template.Value.ToString() == "") || (template.Value.ToString() == "0")))
							{
								float x1 = (float)template.BTE_X1;
								float y1 = (float)template.BTE_Y1;
								float x2 = (float)template.BTE_X2;
								float y2 = (float)template.BTE_Y2;
								
								g.DrawLine(new Pen(Color.Black, 1), x1, y1, x2, y2);
							}
							#endregion "LINE"
							break;
						}
					default:
						break;
				}
			}
		}
		#endregion старая печать
    }
}

