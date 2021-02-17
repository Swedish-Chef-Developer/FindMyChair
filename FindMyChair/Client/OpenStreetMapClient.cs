using OsmSharp;
using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.Utilities;
using Mapsui.Projection;

namespace FindMyChair.Client
{
	public class OpenStreetMapClient : IOpenStreetMapClient
	{
		public async Task<IEnumerable<OsmGeo>> GetuMap()
		{
			return await SeutMap();
		}

		public Map GetMap()
		{
			return SetMap();
		}

		private async Task<IEnumerable<OsmGeo>> SeutMap()
		{

			var mapPath = string.Format("{0}\\Maps\\sweden.osm.pbf", AppDomain.CurrentDomain.BaseDirectory);
			var returnList = new List<OsmGeo>();
			using (var fileStream = File.OpenRead(mapPath))
			{
				// create source stream.
				var source = new PBFOsmStreamSource(fileStream);

				// filter all powerlines and keep all nodes.
				var filtered = from osmGeo in source
							   where osmGeo.Type == OsmSharp.OsmGeoType.Node ||
								(osmGeo.Type == OsmSharp.OsmGeoType.Way && osmGeo.Tags != null && osmGeo.Tags.Contains("power", "line"))
							   select osmGeo;

				// convert to complete stream.
				// WARNING: nodes that are partof powerlines will be kept in-memory.
				//          it's important to filter only the objects you need **before** 
				//          you convert to a complete stream otherwise all objects will 
				//          be kept in-memory.
				var complete = filtered.ToComplete();
				returnList = filtered as List<OsmGeo>;
			}
			return returnList;
		}

		public Map SetMap()
		{
			var map = new Map();
			map.Layers.Add(OpenStreetMap.CreateTileLayer());

			// Get the lon lat coordinates from somewhere (Mapsui can not help you there)
			// 59.3444559,18.0896937
			var centerOfLondonOntario = new Point(59.3444559, 18.0896937);
			// OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
			var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfLondonOntario.X, centerOfLondonOntario.Y);
			// Set the center of the viewport to the coordinate. The UI will refresh automatically
			// Additionally you might want to set the resolution, this could depend on your specific purpose
			map.Home = n => n.NavigateTo(sphericalMercatorCoordinate, map.Resolutions[9]);

			return map;
		}
	}
}