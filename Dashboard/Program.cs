using Raylib_cs;
using System.Numerics;

namespace Dashboard {
	static unsafe class Program {

		static void SetupWindow(int W, int H) {
			Raylib.InitWindow(W, H, "Dashboard");
			Raylib.SetTargetFPS(60);
		}

		static void Main(string[] args) {
			SetupWindow(1200, 600);

			Camera2D Cam = new Camera2D(new Vector2(0, 0), new Vector2(0, 0), 0, 1);

			while (!Raylib.WindowShouldClose()) {
				Raylib.BeginDrawing();
				Raylib.ClearBackground(new Color(40, 40, 40, 255));

				DrawGauge(new Vector2(300, 350), 250);

				DrawGauge(new Vector2(1200 - 300, 350), 250);


				Raylib.EndDrawing();
			}

			Raylib.CloseWindow();
		}

		static void DrawGauge(Vector2 Center, float Radius) {
			float StartAngle = 40;
			float EndAngle = -(180 + 40);

			Raylib.DrawRing(Center, Radius - 2, Radius + 2, StartAngle, EndAngle, 64, Color.White);

			//Raylib.DrawLineV(Center, new Vector2(300, Center.Y), Color.Red);

			DrawBigPointer(Center, 0, Radius);




			Raylib.DrawCircleV(Center, 30, Color.Black);
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

			Matrix3x2 Rot = Matrix3x2.CreateRotation((float)((Angle - 90) * Math.PI / 180), Center);

			for (int i = 0; i < Points.Length; i++) {
				Points[i] = Vector2.Transform(Points[i], Rot);
			}

			fixed (Vector2* PointsArr = Points) {
				Raylib.DrawTriangleStrip(PointsArr, Points.Length, Color.Red);
			}
		}
	}
}
