using Raylib_cs;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace Dashboard {
	static unsafe class DashboardProgram {
		static VehicleData VehData = new VehicleData();

		static void Main(string[] args) {
			DashboardEngine DEngine = new DashboardEngine();
			DEngine.SetupWindow(1200, 600);

			/*Camera2D Cam = new Camera2D(new Vector2(0, 0), new Vector2(0, 0), 0, 1);

			int[] Angles = new int[8];
			Random Rnd = new Random();


			for (int i = 0; i < Angles.Length; i++) {
				Angles[i] = Rnd.Next(10, 91);
			}*/

			Stopwatch SWatch = Stopwatch.StartNew();

			Thread TestThread = new Thread(() => {
				while (true) {
					Thread.Sleep(1000);

					if (VehData.ShowBootSequence) {
						VehData.ShowBootSequence = false;

						VehData.ShowBootSequence = false;
						VehData.Engine_CheckEngine = false;
						VehData.Engine_Abs = false;
						VehData.Engine_StabilityControl = false;
						VehData.Engine_StabilityControlOff = false;
						VehData.Engine_Oil = false;
						VehData.Engine_Battery = false;
					}

					if (!VehData.Engine_CheckEngine) {
						if (SWatch.Elapsed.Seconds > 5)
							VehData.Engine_CheckEngine = true;
					}

					VehData.KmH += 60;
					if (VehData.KmH > 220)
						VehData.KmH = 0;

					VehData.CLT += 10;
					if (VehData.CLT > 130)
						VehData.CLT = 50;

					VehData.Fuel += 50;
					if (VehData.Fuel > 100)
						VehData.Fuel = 0;

					if (VehData.RPM >= 4000 && VehData.RPM < 4300)
						VehData.RPM += 50;
					else
						VehData.RPM += 1000;

					if (VehData.RPM > 8000)
						VehData.RPM = 1000;

					Console.WriteLine("RPM: {0}; KMH: {1}; Fuel: {2}; CLT: {3}", VehData.RPM, VehData.KmH, VehData.Fuel, VehData.CLT);
				}
			});
			TestThread.IsBackground = true;
			TestThread.Start();


			double LastTime = 0;


			while (!Raylib.WindowShouldClose()) {
				double CurSeconds = SWatch.Elapsed.TotalSeconds;
				Update((float)(CurSeconds - LastTime));
				LastTime = CurSeconds;

				DEngine.Draw(VehData);
			}

			Raylib.CloseWindow();
		}

		static void Update(float Dt) {
			CalculateCorrection(ref VehData.Cur_KmH, VehData.KmH, Dt, 150);
			CalculateCorrection(ref VehData.Cur_RPM, VehData.RPM, Dt, 500);

			CalculateCorrection(ref VehData.Cur_CLT, VehData.CLT, Dt, 60);
			CalculateCorrection(ref VehData.Cur_Fuel, VehData.Fuel, Dt, 75);
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
