using Raylib_cs;

using System.Numerics;

namespace Dashboard {
	delegate string DisplayValueFunc(float Value);

	static unsafe class Gauge {
		public static void RenderGauge(DashboardEngine Dashboard, Vector2 Center, float Radius, float GaugeRedLine, int DisplaySegments, float GaugeMinValue, float GaugeMaxValue, int GaugeStep, float GaugeDisplayValue, float GaugeRealValue, string Txt, int[] RedMarkers = null, DisplayValueFunc DisplayFunc = null) {
			// Calculated
			float GaugeRange = GaugeMaxValue - GaugeMinValue;
			float G1_ToValue = (100.0f / DisplaySegments);

			// Draw small chevrons
			for (int j = 0; j < DisplaySegments; j++) {
				float BaseAngle = G1_ToValue * j;

				for (int i = 0; i < 10; i++) {
					Color Clr = Color.White;
					float RadiusOffset = -2;

					if (i == 5) {
						Clr = Color.Gray;
						RadiusOffset = -7;
					}

					if ((BaseAngle + (G1_ToValue / 10.0f) * i) / G1_ToValue >= GaugeRedLine) {
						Clr = Color.Red;
					}

					DrawGaugeText(Dashboard, Center, BaseAngle + (G1_ToValue / 10.0f) * i, Radius + RadiusOffset, 3, 6, null, 0, Clr);
				}
			}

			DrawGauge(Center, Radius, Color.White, Color.Red, G1_ToValue * GaugeRedLine);

			// Draw thick chevrons
			for (int i = 0; i < DisplaySegments + 1; i++) {
				int ChevNum = (int)(GaugeMinValue + (GaugeStep * i));
				string ChevTxt = ChevNum.ToString();


				Color Clr = Color.White;

				if (i >= GaugeRedLine) {
					Clr = Color.Red;
				}

				DrawGaugeText(Dashboard, Center, G1_ToValue * i, Radius, 5, 16, ChevTxt, 36, Clr);
			}

			// Red markers
			if (RedMarkers != null) {
				for (int i = 0; i < RedMarkers.Length; i++) {
					Color Clr = Color.Red;
					float Angle = RedMarkers[i] / GaugeMaxValue * 100;

					DrawGaugeText(Dashboard, Center, Angle, Radius - 5, 6, 11, null, 0, Clr);
				}
			}

			DrawCenterText(Dashboard, Center + new Vector2(0, 60), Txt, 28, 0, new Color(255, 255, 255, 150));

			if (DisplayFunc != null)
				DrawCenterText2(Dashboard, Center + new Vector2(0, 100), DisplayFunc(GaugeRealValue), 28, 0, new Color(255, 255, 255, 255));

			float DisplayPerc = 0;

			if (GaugeDisplayValue >= GaugeMinValue && GaugeDisplayValue <= GaugeMaxValue) {
				DisplayPerc = (GaugeDisplayValue / GaugeMaxValue) * 100;
			}


			DrawGaugePointer(Center, DisplayPerc, Radius);
		}

		static void DrawGaugePointer(Vector2 Center, float Value, float Radius) {
			DrawBigPointer(Center, Value, Radius);
			Raylib.DrawCircleV(Center, 30, Color.DarkGray);
		}

		static void DrawGauge(Vector2 Center, float Radius, Color Color1, Color Color2, float SplitPercentage) {
			float Thickness = 6;
			float StartAngle = 40;
			float EndAngle = -(180 + 40);
			float AngleRange = StartAngle - EndAngle;

			float MiddleEndAngle = StartAngle - (AngleRange * (1.0f - (SplitPercentage / 100)));

			if (SplitPercentage > 0 && SplitPercentage < 100) {
				Raylib.DrawRing(Center, Radius - (Thickness / 2), Radius + (Thickness / 2), MiddleEndAngle, EndAngle, 64, Color1);
				Raylib.DrawRing(Center, Radius - (Thickness / 2), Radius + (Thickness / 2), StartAngle, MiddleEndAngle, 64, Color2);

			} else {
				Raylib.DrawRing(Center, Radius - (Thickness / 2), Radius + (Thickness / 2), StartAngle, EndAngle, 64, Color1);
			}

			float InnerThickness = 1;
			float InnerOffset = 13;

			if (SplitPercentage > 0 && SplitPercentage < 100) {
				Raylib.DrawRing(Center, Radius - InnerOffset - InnerThickness, Radius - InnerOffset, MiddleEndAngle, EndAngle, 64, Color.Gray);
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


		static void DrawCenterText2(DashboardEngine Dashboard, Vector2 Pos, string Txt, float FontSize, float Angle, Color Clr) {
			Vector2 TxtSize = Raylib.MeasureTextEx(Dashboard.MonoFont, Txt, FontSize, 0);
			Raylib.DrawTextPro(Dashboard.MonoFont, Txt, Pos, TxtSize / 2, Angle, FontSize, 0, Clr);
		}

		static void DrawCenterText(DashboardEngine Dashboard, Vector2 Pos, string Txt, float FontSize, float Angle, Color Clr) {
			Vector2 TxtSize = Raylib.MeasureTextEx(Dashboard.Font, Txt, FontSize, 0);
			Raylib.DrawTextPro(Dashboard.Font, Txt, Pos, TxtSize / 2, Angle, FontSize, 0, Clr);
		}

		static void DrawGaugeText(DashboardEngine Dashboard, Vector2 Center, float Angle, float Radius, float NotchWidth, float NotchHeight, string Text, float FontSize, Color Clr) {
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
				DrawCenterText(Dashboard, TextPos, Text, FontSize, Ang - 90, Clr);
			}

			//Raylib.DrawCircleV(TextPos, 10, Color.Green);
		}

		static float Rad(float Deg) {
			return (float)(Deg * Math.PI / 180);
		}
	}
}
