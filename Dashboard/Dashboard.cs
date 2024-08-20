using Raylib_cs;

using System.Numerics;
using System.Runtime.ConstrainedExecution;

namespace Dashboard {
	static unsafe class Dashboard {
		const bool EnableFiltering = true;

		static Font Font;

		static float Rad(float Deg) {
			return (float)(Deg * Math.PI / 180);
		}

		static void SetupWindow(int W, int H) {
			if (EnableFiltering)
				Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);

			Raylib.InitWindow(W, H, "Dashboard");
			Raylib.SetTargetFPS(60);

			Font = Raylib.LoadFont("data/fonts/abeezee.ttf");

			if (EnableFiltering)
				Raylib.SetTextureFilter(Font.Texture, TextureFilter.Trilinear);
		}

		static void Main(string[] args) {
			SetupWindow(1200, 600);

			Camera2D Cam = new Camera2D(new Vector2(0, 0), new Vector2(0, 0), 0, 1);

			int[] Angles = new int[8];
			Random Rnd = new Random();

			for (int i = 0; i < Angles.Length; i++) {
				Angles[i] = Rnd.Next(10, 91);
			}

			while (!Raylib.WindowShouldClose()) {
				Raylib.BeginDrawing();
				Raylib.ClearBackground(new Color(40, 40, 40, 255));

				Vector2 Center = new Vector2(300, 350);
				float Radius = 250;


				float G1_ToValue = (100.0f / 8.0f);



				// Draw small chevrons
				for (int j = 0; j < 8; j++) {
					float BaseAngle = (100.0f / 8.0f) * j;
					for (int i = 0; i < 10; i++) {
						Color Clr = Color.White;

						if (i == 5)
							Clr = Color.Gray;

						if ((BaseAngle + (G1_ToValue / 10.0f) * i) / G1_ToValue > 6.5f) {
							Clr = Color.Red;
						}

						DrawGaugeText(Center, BaseAngle + (G1_ToValue / 10.0f) * i, Radius, 2, 8, null, 0, Clr);
					}
				}

				DrawGauge(Center, Radius, Color.White, Color.Red, G1_ToValue * 6.5f);

				// Draw thick chevrons
				for (int i = 0; i < 9; i++) {
					string Txt = i.ToString();
					Color Clr = Color.White;

					if (i > 6) {
						Clr = Color.Red;
					}

					DrawGaugeText(Center, G1_ToValue * i, Radius, 5, 16, Txt, 32, Clr);
				}

				DrawCenterText(Center + new Vector2(0, 60), "RPM x 1000", 28, 0, new Color(255, 255, 255, 150));
				DrawGaugePointer(Center, G1_ToValue * 1.2f, Radius);

				// 4, 12

				/*for (int i = 0; i < Angles.Length; i++) {
					Color Clr = Color.White;

					if (Angles[i] % 5 == 0)
						Clr = Color.Red;

					DrawGaugeText(Center, Angles[i], Radius, Angles[i].ToString(), Clr);
				}*/

				//DrawGaugeText(Center, 0, Radius, "0");
				//DrawGaugeText(Center, 50, Radius, "100");

				DrawGauge(new Vector2(1200 - 300, 350), 250, Color.White, Color.White, 0);


				Raylib.EndDrawing();
			}

			Raylib.CloseWindow();
		}

		static void DrawGaugePointer(Vector2 Center, float Value, float Radius) {
			DrawBigPointer(Center, Value, Radius);
			Raylib.DrawCircleV(Center, 30, Color.Black);
		}

		static void DrawGauge(Vector2 Center, float Radius, Color Color1, Color Color2, float SplitPercentage) {
			float StartAngle = 40;
			float EndAngle = -(180 + 40);
			float AngleRange = StartAngle - EndAngle;

			float MiddleEndAngle = StartAngle - (AngleRange * (1.0f - (SplitPercentage / 100)));

			if (SplitPercentage > 0 && SplitPercentage < 100) {
				Raylib.DrawRing(Center, Radius - 2, Radius + 2, MiddleEndAngle, EndAngle, 64, Color1);
				Raylib.DrawRing(Center, Radius - 2, Radius + 2, StartAngle, MiddleEndAngle, 64, Color2);

			} else {
				Raylib.DrawRing(Center, Radius - 2, Radius + 2, StartAngle, EndAngle, 64, Color1);
			}

			float InnerThickness = 1;
			float InnerOffset = 13;

			if (SplitPercentage > 0 && SplitPercentage < 100) {
				Raylib.DrawRing(Center, Radius - InnerOffset - InnerThickness, Radius - InnerOffset, MiddleEndAngle, EndAngle, 64, Color1);
				Raylib.DrawRing(Center, Radius - InnerOffset - InnerThickness, Radius - InnerOffset, StartAngle, MiddleEndAngle, 64, Color2);
			} else {
				Raylib.DrawRing(Center, Radius - InnerOffset - InnerThickness, Radius - InnerOffset, StartAngle, EndAngle, 64, Color.Gray);
			}
			//Raylib.DrawLineV(Center, new Vector2(300, Center.Y), Color.Red);

			//Raylib.DrawTextEx(Font, "1234567890", Center, 24, 0, Color.White);
		}

		static void DrawBigPointer(Vector2 Center, float Angle, float Radius) {
			Angle = Angle * (260.0f / 100.0f);

			Vector2[] Points = [
				new Vector2(Center.X - 5, Center.Y),
				new Vector2(Center.X - 2, Center.Y + Radius),
				new Vector2(Center.X + 5, Center.Y),

				new Vector2(Center.X - 2, Center.Y + Radius),
				new Vector2(Center.X + 2, Center.Y + Radius),
				new Vector2(Center.X + 5, Center.Y)
			];

			Matrix3x2 Rot = Matrix3x2.CreateRotation(Rad(Angle + 50), Center);

			for (int i = 0; i < Points.Length; i++) {
				Points[i] = Vector2.Transform(Points[i], Rot);
			}

			fixed (Vector2* PointsArr = Points) {
				Raylib.DrawTriangleStrip(PointsArr, Points.Length, Color.Red);
			}
		}


		static void DrawCenterText(Vector2 Pos, string Txt, float FontSize, float Angle, Color Clr) {
			Vector2 TxtSize = Raylib.MeasureTextEx(Font, Txt, FontSize, 0);
			Raylib.DrawTextPro(Font, Txt, Pos, TxtSize / 2, Angle, FontSize, 0, Clr);
		}

		static void DrawGaugeText(Vector2 Center, float Angle, float Radius, float NotchWidth, float NotchHeight, string Text, float FontSize, Color Clr) {
			Angle = Angle * (260.0f / 100.0f);

			Vector2[] Points = [
				new Vector2(Center.X - NotchWidth / 2, Center.Y + Radius - NotchHeight),
				new Vector2(Center.X - NotchWidth / 2, Center.Y + Radius),
				new Vector2(Center.X + NotchWidth / 2, Center.Y + Radius - NotchHeight),

				new Vector2(Center.X - NotchWidth / 2, Center.Y + Radius),
				new Vector2(Center.X + NotchWidth / 2, Center.Y + Radius),
				new Vector2(Center.X + NotchWidth / 2, Center.Y + Radius - NotchHeight)
			];


			Matrix3x2 Rot = Matrix3x2.CreateRotation(Rad(Angle + 50), Center);

			Vector2 TextPos = Center + new Vector2(0, Radius - NotchHeight - 20);
			TextPos = Vector2.Transform(TextPos, Rot);

			for (int i = 0; i < Points.Length; i++) {
				Points[i] = Vector2.Transform(Points[i], Rot);
			}

			fixed (Vector2* PointsArr = Points) {
				Raylib.DrawTriangleStrip(PointsArr, Points.Length, Clr);
			}

			if (Text != null) {
				float xDiff = Center.X - TextPos.X;
				float yDiff = Center.Y - TextPos.Y;
				float Ang = (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);

				//TextPos = new Vector2(0, 0) + Center;
				DrawCenterText(TextPos, Text, FontSize, Ang - 90, Clr);
			}

			//Raylib.DrawCircleV(TextPos, 10, Color.Green);
		}
	}
}
