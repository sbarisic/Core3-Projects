using Raylib_cs;

using System.Numerics;

namespace Dashboard {
	static unsafe class Utils {
		public static float Rad(float Deg) {
			return (float)(Deg * Math.PI / 180);
		}
	}
}
