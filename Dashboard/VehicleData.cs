using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard {
	class GaugeUnit<T> {
		public T Value;

		public T GaugeValue;
		public float Inertia;

		public GaugeUnit(float Inertia, T Value) {
			this.Inertia = Inertia;
			this.Value = Value;
			this.GaugeValue = Value;
		}

		public void Set(T Val) {
			GaugeValue = Val;
			Value = Val;
		}

		public void Update(float Dt) {

		}
	}

	class VehicleData {
		public GaugeUnit<float> Bullshit = new GaugeUnit<float>(150, 0);

		// Actual values
		public float KmH;
		public float RPM;
		public float CLT;
		public float Fuel;

		// Used for display smoothing
		public float Cur_KmH;
		public float Cur_RPM;
		public float Cur_CLT;
		public float Cur_Fuel;

		// Indicator flags
		public bool ShowBootSequence = true;
		public bool Engine_CheckEngine = true;
		public bool Engine_Abs = true;
		public bool Engine_StabilityControl = true;
		public bool Engine_StabilityControlOff = true;
		public bool Engine_Oil = true;
		public bool Engine_Battery = true;
	}
}
