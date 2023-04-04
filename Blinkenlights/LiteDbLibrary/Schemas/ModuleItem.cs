using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteDbLibrary.Schemas
{
	public class ModuleItem : ILiteDbObject
	{
		public int Id { get; set; }

		public string Name { get; init; }
		public int RowStart { get; init; }
		public int RowEnd { get; init; }
		public int ColStart { get; init; }
		public int ColEnd { get; init; }

		public ModuleItem(string name, int row, int col, int colSpan = 1, int rowSpan = 1)
		{
			Name = name;
			RowStart = row;
			RowEnd = RowStart + rowSpan;
			ColStart = col;
			ColEnd = ColStart + colSpan;
		}

		public string ToGridArea()
		{
			return $"grid-area: {RowStart} / {ColStart} / {RowEnd} / {ColEnd};";
		}

		public bool SameIndex(ILiteDbObject other)
		{
			return string.Equals(this.Name, (other as ModuleItem).Name);
		}
	}
}
