using Raylib_cs;

using System.Numerics;

namespace Dashboard {
	static unsafe class BarGauge {
		const float SegmentWidth = 16;
		const float SegmentDrop = 4;

		const float BoxWidth = SegmentWidth + 2;
		const float BoxHeight = 8;
		const float BoxOffset = 4;

		const float OffsetBetweenSegments = 20;

		public static Vector2 RenderGauge(DashboardEngine Dashboard, Vector2 Start, float Min, float Max, float Value, string LeftText, string CenterText, string RightText, string Title, Color[] Colors, int[] Bars) {
			int SegmentCount = 8;


			Vector2 EndPoint = Vector2.Zero;

			float Range = Max - Min;
			float SegmentRange = Range / SegmentCount;

			//Value = Value - Min;

			for (int i = 0; i < SegmentCount; i++) {
				Vector2 Offset = new Vector2(OffsetBetweenSegments, 0);

				float SegmentValue = SegmentRange * i + Min;
				float NextSegmentValue = SegmentRange * (i + 1) + Min;

				Color Clr = Colors[i];
				EndPoint = Vector2.Max(EndPoint, DrawSegment(Start + Offset * i, Clr, Value > SegmentValue, Value > NextSegmentValue, Bars[i]));
			}

			float SmallFontSize = 18;
			float FontSize = 28;
			Vector2 TextSize = Raylib.MeasureTextEx(Dashboard.Font, LeftText, FontSize, 0);
			Raylib.DrawTextEx(Dashboard.Font, LeftText, Start - new Vector2(2, TextSize.Y), FontSize, 0, Color.White);

			TextSize = Raylib.MeasureTextEx(Dashboard.Font, CenterText, FontSize, 0);
			Raylib.DrawTextEx(Dashboard.Font, CenterText, Start - new Vector2(-OffsetBetweenSegments * (SegmentCount / 2) + (TextSize.X / 2) + 6, TextSize.Y), FontSize, 0, Color.White);

			TextSize = Raylib.MeasureTextEx(Dashboard.Font, RightText, FontSize, 0);
			Raylib.DrawTextEx(Dashboard.Font, RightText, Start - new Vector2(-OffsetBetweenSegments * (SegmentCount - 1) + (TextSize.X / 2) + 6, TextSize.Y), FontSize, 0, Color.White);

			TextSize = Raylib.MeasureTextEx(Dashboard.Font, Title, SmallFontSize, 0);
			Raylib.DrawTextEx(Dashboard.Font, Title, Start - new Vector2((-OffsetBetweenSegments * (SegmentCount / 2)) + TextSize.X / 2, TextSize.Y + FontSize), SmallFontSize, 0, Color.LightGray);

			return EndPoint - Start;
		}

		static Vector2 DrawSegment(Vector2 Pos, Color Clr, bool Full, bool NextFull, int Bar) {
			Vector2 Start = Pos;
			Vector2 End = Pos + new Vector2(SegmentWidth, 0);
			Vector2 RectStart = Start + new Vector2(-1, SegmentDrop + BoxOffset);

			const float LineThick = 2;

			Raylib.DrawLineEx(Start, End, LineThick, Clr);
			Raylib.DrawLineEx(Start + new Vector2(0, SegmentDrop), Start, LineThick, Clr);
			Raylib.DrawLineEx(End, End + new Vector2(0, SegmentDrop), LineThick, Clr);

			if (Full) {
				if (Bar == 1 && NextFull)
					Clr = Color.White;
				if (Bar > 1)
					Clr = Color.White;

				Raylib.DrawRectangleV(RectStart, new Vector2(BoxWidth, BoxHeight), Clr);
			}

			return RectStart + new Vector2(BoxWidth, BoxHeight);
		}
	}
}
