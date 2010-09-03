using System;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Globalization;

namespace ImageMapFromSvg
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string filename = args[0];
			
			Console.WriteLine (	"loading map data from file " + filename);
			
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("x", doc.DocumentElement.NamespaceURI);
			
			Console.WriteLine ("<map>");
			
			foreach(XmlNode node in doc.SelectNodes("//x:path", nsmgr) )
			{
				var d = node.Attributes["d"].InnerText.Split(' ');
				Console.WriteLine ("<area title='{0}' shape='poly' coords='{1}'></area>", node.Attributes["id"].InnerText, BuildCoords(d) );
			}
			Console.WriteLine ("</map>");
		}
		
		public static string BuildCoords( string[] coords )
		{
			StringBuilder result = new StringBuilder();
			string firstPoint = null;
			Point point = new Point(0,0);
			bool isRelative = true;
			foreach( string s in coords )
			{
				switch(s[0] )
				{
				case 'm':
				case 'l':
					isRelative = true;
					break;
				case 'M':
				case 'L':
					isRelative = false;
					break;
				case 'z':
					break;
				default:
					Point next = PointFromString(s);
					if( isRelative )
						point = new Point(point.X + next.X, point.Y + next.Y);
					else
						point = next;
					if( firstPoint == null )
						firstPoint = PrintPoint(point,false);
					result.Append(PrintPoint(point,true));
					break;
				}
			}
			result.Append(firstPoint);
			return result.ToString();
		}
		
		private static string PrintPoint(Point p, bool trailComma)
		{
			return string.Format("{0},{1}{2}", p.X,p.Y, trailComma ? ", " : "");
		}
		
		private static Point PointFromString( string input )
		{
			string[] coords = input.Split(',');
			return new Point( GetNumber( coords[0]) , GetNumber(coords[1]) );
		}
		
		private static int GetNumber(string n)
		{
			return (int)Math.Round(float.Parse(n, System.Globalization.NumberStyles.Any));
		}
	}
}

