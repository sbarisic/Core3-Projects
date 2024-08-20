using Raylib_cs;

using System.Numerics;
using System.Runtime.ConstrainedExecution;

namespace Dashboard {
	static unsafe class Dashboard {
		const bool EnableFiltering = true;

		public static Font Font;

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

				Gauge.RenderGauge(Center, Radius, 6.5f, 8, 0, 8, 1, 0.9f, "x1000/min");


				Center = new Vector2(1200 - 300, 350);
				Gauge.RenderGauge(Center, Radius, 100, 11, 0, 220, 20, 150, "km/h", [30, 50]);


				Raylib.EndDrawing();
			}

			Raylib.CloseWindow();
		}
	}
}
