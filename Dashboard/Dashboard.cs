using Raylib_cs;

using System.Diagnostics;
using System.Numerics;
using System.Runtime.ConstrainedExecution;

namespace Dashboard {
	static unsafe class Dashboard {
		const bool EnableFiltering = true;

		public static Font Font;
		public static Font MonoFont;
		static Stopwatch SWatch;

		static Thread UpdateThread;

		static void SetupWindow(int W, int H) {
			if (EnableFiltering)
				Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);

			Raylib.SetConfigFlags(ConfigFlags.VSyncHint);
			Raylib.SetTargetFPS(260);

			Raylib.InitWindow(W, H, "Dashboard");


			Font = Raylib.LoadFont("data/fonts/abeezee.ttf");
			MonoFont = Raylib.LoadFont("data/fonts/VeraMono.ttf");

			if (EnableFiltering) {
				Raylib.SetTextureFilter(Font.Texture, TextureFilter.Trilinear);
				Raylib.SetTextureFilter(MonoFont.Texture, TextureFilter.Trilinear);
			}
		}

		static void Main(string[] args) {
			SetupWindow(1200, 600);

			Camera2D Cam = new Camera2D(new Vector2(0, 0), new Vector2(0, 0), 0, 1);

			int[] Angles = new int[8];
			Random Rnd = new Random();

			for (int i = 0; i < Angles.Length; i++) {
				Angles[i] = Rnd.Next(10, 91);
			}

			SWatch = Stopwatch.StartNew();

			UpdateThread = new Thread(() => {
				double LastTime = 0;
				while (true) {
					double CurSeconds = SWatch.Elapsed.TotalSeconds;

					Update((float)(CurSeconds - LastTime));

					LastTime = CurSeconds;
					Thread.Sleep(1000 / 60);
				}
			});

			UpdateThread.IsBackground = true;
			UpdateThread.Start();

			Thread TestThread = new Thread(() => {
				while (true) {
					KmH += 60;
					if (KmH > 220)
						KmH = 0;

					CLT += 10;
					if (CLT > 130)
						CLT = 50;

					Fuel += 50;
					if (Fuel > 100)
						Fuel = 0;

					if (RPM >= 4000 && RPM < 4300)
						RPM += 50;
					else
						RPM += 1000;

					if (RPM > 8000)
						RPM = 1000;

					Console.WriteLine("RPM: {0}; KMH: {1}; Fuel: {2}; CLT: {3}", RPM, KmH, Fuel, CLT);

					Thread.Sleep(1000);
				}
			});
			TestThread.IsBackground = true;
			TestThread.Start();

			while (!Raylib.WindowShouldClose()) {
				Raylib.BeginDrawing();
				Raylib.ClearBackground(new Color(20, 20, 20, 255));

				//RPM = (SWatch.Elapsed.Seconds % (8000 / 500)) * 500;

				Vector2 Center = new Vector2(300, 350);
				float Radius = 250;

				Gauge.RenderGauge(Center, Radius, 6.5f, 8, 0, 8, 1, Cur_RPM / 1000.0f, RPM, "x1000/min", null, (Val) => {
					return ((int)Val).ToString() + " rpm";
				});

				Center = new Vector2(1200 - 300, 350);
				Gauge.RenderGauge(Center, Radius, 100, 11, 0, 220, 20, Cur_KmH, KmH, "km/h", [30, 50], (Val) => {
					return ((int)Val).ToString();
				});

				Center = new Vector2(400, 60);
				Vector2 Sz = BarGauge.RenderGauge(Center, 0, 100, Cur_Fuel, "0", "1/2", "1", "Fuel",
					[Color.Red, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White],
					[1, 0, 0, 0, 0, 0, 0, 0]
				);

				Center = new Vector2(1200 - 400 - Sz.X, 60);
				BarGauge.RenderGauge(Center, 50, 130, Cur_CLT, "50", "90", "130", "Coolant Temp",
				   [Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.Red, Color.Red],
				   [0, 0, 0, 0, 0, 0, 0, 0]
			   );


				Raylib.EndDrawing();
			}

			Raylib.CloseWindow();
		}

		static float KmH;
		static float RPM;
		static float CLT;
		static float Fuel;

		static float Cur_KmH;
		static float Cur_RPM;
		static float Cur_CLT;
		static float Cur_Fuel;

		static void Update(float Dt) {
			CalculateCorrection(ref Cur_KmH, KmH, Dt, 150);
			CalculateCorrection(ref Cur_RPM, RPM, Dt, 500);

			CalculateCorrection(ref Cur_CLT, CLT, Dt, 60);
			CalculateCorrection(ref Cur_Fuel, Fuel, Dt, 75);

		}

		static void CalculateCorrection(ref float Cur_Val, float Val, float Dt, float Speed) {
			float Dif = Val - Cur_Val;
			float AbsDif = Math.Abs(Dif);
			float Dir = (Dif > 0) ? 1 : -1;

			Speed = Speed * (Math.Min(AbsDif, 2000) + 40) * Dt;

			float Phi = 0.1f;

			if (AbsDif > Phi) {
				float Correction = Dir * Speed * Dt;

				if (Math.Abs(Correction) > AbsDif)
					Correction = Dif;

				Cur_Val = Cur_Val + Correction;
			} else
				Cur_Val = Val;
		}
	}
}
