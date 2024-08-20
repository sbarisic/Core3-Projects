using Raylib_cs;
using System.Numerics;

namespace Dashboard {
	static unsafe class Program {
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

			while (!Raylib.WindowShouldClose()) {
				Raylib.BeginDrawing();
				Raylib.ClearBackground(new Color(40, 40, 40, 255));

				DrawGauge(new Vector2(300, 350), 250);
				DrawGaugeText(new Vector2(300, 350), 90, 250, "69");

				DrawGauge(new Vector2(1200 - 300, 350), 250);


				Raylib.EndDrawing();
			}

			Raylib.CloseWindow();
		}

		static void DrawGauge(Vector2 Center, float Radius) {
			float StartAngle = 40;
			float EndAngle = -(180 + 40);
			float AngleRange = StartAngle - EndAngle;

			Raylib.DrawRing(Center, Radius - 2, Radius + 2, StartAngle, EndAngle, 64, Color.White);

			//Raylib.DrawLineV(Center, new Vector2(300, Center.Y), Color.Red);

			DrawBigPointer(Center, 0, Radius);


			Raylib.DrawCircleV(Center, 30, Color.Black);

			//Raylib.DrawTextEx(Font, "1234567890", Center, 24, 0, Color.White);
		}

		static void DrawBigPointer(Vector2 Center, float Angle, float Radius) {
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

		static void DrawGaugeText(Vector2 Center, float Angle, float Radius, string Text) {
			const float Width = 2;
			const float Height = 12;

			Vector2[] Points = [
				new Vector2(Center.X - Width, Center.Y + Radius - Height),
				new Vector2(Center.X - Width, Center.Y + Radius),
				new Vector2(Center.X + Width, Center.Y + Radius - Height),

				new Vector2(Center.X - Width, Center.Y + Radius),
				new Vector2(Center.X + Width, Center.Y + Radius),
				new Vector2(Center.X + Width, Center.Y + Radius - Height)
			];


			Matrix3x2 Rot = Matrix3x2.CreateRotation(Rad(Angle + 50), Center);

			//Vector2 TextPos = Center + new Vector2(0, Radius - Height - 5);
			//TextPos = Vector2.Transform(TextPos, Rot);

			for (int i = 0; i < Points.Length; i++) {
				Points[i] = Vector2.Transform(Points[i], Rot);
			}

			fixed (Vector2* PointsArr = Points) {
				Raylib.DrawTriangleStrip(PointsArr, Points.Length, Color.White);
			}

			Vector2 TextPos = new Vector2(0, 0) + Center;
			Raylib.DrawTextPro(Font, Text, TextPos, new Vector2(0, 0), 0, 22, 0, Color.White);
		}
	}
}
