using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Drawing;
using System.Runtime.Serialization;

namespace GPSCheck
{
	public class GmapMarkerWithLabel : GMapMarker, ISerializable
	{
		private Font font;

		private GMarkerGoogle innerMarker;

		public string Caption;

		public GmapMarkerWithLabel(PointLatLng p, string caption, GMarkerGoogleType type)
			: base(p)
		{
			font = new Font("Arial", 8f);
			innerMarker = new GMarkerGoogle(p, type);
			Caption = caption;
		}

		public override void OnRender(Graphics g)
		{
			if (innerMarker != null)
			{
				innerMarker.OnRender(g);
			}
			g.DrawString(Caption, font, Brushes.Black, new PointF(0f, (float)innerMarker.Size.Height));
		}

		public override void Dispose()
		{
			if (innerMarker != null)
			{
				innerMarker.Dispose();
				innerMarker = null;
			}
			base.Dispose();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetObjectData(info, context);
		}

		protected GmapMarkerWithLabel(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
