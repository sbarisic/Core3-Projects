using Raylib_cs;

using System.Diagnostics;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.Arm;
using System.Security;
using System.Text.RegularExpressions;

namespace Dashboard {
	class DashboardEngine {
		const bool EnableFiltering = true;
		const float IconScale = 0.25f;

		public Font Font;
		public Font MonoFont;

		public Texture2D Tex_CheckEngine;
		public Texture2D Tex_Abs;
		public Texture2D Tex_StabilityControl;
		public Texture2D Tex_StabilityControlOff;
		public Texture2D Tex_Oil;
		public Texture2D Tex_Battery;

		Texture2D LoadTex(string FilePath) {
			Texture2D Tex = Raylib.LoadTexture(FilePath);
			Raylib.SetTextureFilter(Tex, TextureFilter.Trilinear);
			return Tex;
		}

		Font LoadFont(string FontPath) {
			Font F = Raylib.LoadFont(FontPath);

			if (EnableFiltering)
				Raylib.SetTextureFilter(F.Texture, TextureFilter.Trilinear);

			return F;
		}

		void LoadTextures() {
			// Icons from https://www.germaingm.com/gm-dashboard-warning-lights-guide/
			Tex_CheckEngine = LoadTex("data/icons/checkengine.png");
			Tex_Abs = LoadTex("data/icons/abs.png");
			Tex_StabilityControl = LoadTex("data/icons/stability_control.png");
			Tex_StabilityControlOff = LoadTex("data/icons/stability_control_off.png");
			Tex_Oil = LoadTex("data/icons/oil_lamp.png");
			Tex_Battery = LoadTex("data/icons/battery.png");
		}

		void LoadFonts() {
			Font = LoadFont("data/fonts/abeezee.ttf");
			MonoFont = LoadFont("data/fonts/VeraMono.ttf");
		}

		public void SetupWindow(int W, int H) {
			if (EnableFiltering)
				Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);

			Raylib.SetConfigFlags(ConfigFlags.VSyncHint);

			int CurrentMon = Raylib.GetCurrentMonitor();
			int MonitorRefresh = Raylib.GetMonitorRefreshRate(CurrentMon);
			Raylib.SetTargetFPS(MonitorRefresh);

			Raylib.InitWindow(W, H, "Dashboard");

			LoadTextures();
			LoadFonts();
		}

		public void Draw(VehicleData Dat) {
			Raylib.BeginDrawing();
			Raylib.ClearBackground(new Color(20, 20, 20, 255));

			Raylib.DrawFPS(0, 0);

			if (Dat.Engine_CheckEngine)
				Raylib.DrawTextureEx(Tex_CheckEngine, new Vector2(150, 500), 0, IconScale, Color.White);

			if (Dat.Engine_Oil)
				Raylib.DrawTextureEx(Tex_Oil, new Vector2(200, 470), 0, IconScale, Color.White);

			if (Dat.Engine_StabilityControl)
				Raylib.DrawTextureEx(Tex_StabilityControl, new Vector2(340, 220), 0, IconScale, Color.White);

			if (Dat.Engine_StabilityControlOff)
				Raylib.DrawTextureEx(Tex_StabilityControlOff, new Vector2(220, 225), 0, IconScale, Color.White);

			if (Dat.Engine_Battery)
				Raylib.DrawTextureEx(Tex_Battery, new Vector2(340, 470), 0, IconScale, Color.White);

			//RPM = (SWatch.Elapsed.Seconds % (8000 / 500)) * 500;

			Vector2 Center = new Vector2(300, 350);
			float Radius = 250;

			Gauge.RenderGauge(this, Center, Radius, 6.5f, 8, 0, 8, 1, Dat.Cur_RPM / 1000.0f, Dat.RPM, "x1000/min", null, (Val) => {
				return ((int)Val).ToString();
			});

			Center = new Vector2(1200 - 300, 350);
			Gauge.RenderGauge(this, Center, Radius, 100, 11, 0, 220, 20, Dat.Cur_KmH, Dat.KmH, "km/h", [30, 50], (Val) => {
				return ((int)Val).ToString();
			});

			Center = new Vector2(400, 60);
			Vector2 Sz = BarGauge.RenderGauge(this, Center, 0, 100, Dat.Cur_Fuel, "0", "1/2", "1", "Fuel",
				[Color.Red, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White],
				[1, 0, 0, 0, 0, 0, 0, 0]
			);

			Center = new Vector2(1200 - 400 - Sz.X, 60);
			BarGauge.RenderGauge(this, Center, 50, 130, Dat.Cur_CLT, "50", "90", "130", "Coolant Temp",
			   [Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.Red, Color.Red],
			   [0, 0, 0, 0, 0, 0, 0, 0]
		   );

			Raylib.EndDrawing();
		}
	}
}
