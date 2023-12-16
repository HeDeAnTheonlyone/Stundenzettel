using OfficeOpenXml;
using OfficeOpenXml.Style;

public static class ExcelRangeExtension
{
	public static ExcelRange FormatCell
	(
		this ExcelRange cell,
		string value,
		bool bold = false
	)
	{

		cell.Value = value;
		cell.Style.Font.Bold = bold;

		Border border = cell.Style.Border;
		border.Top.Style = border.Bottom.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

		return cell;
	}



	public static ExcelRange FormatCell
	(
		this ExcelRange cell,
		bool merge,
		string value,
		bool bold = false,
		ExcelHorizontalAlignment hAlignment = ExcelHorizontalAlignment.Left
	)
	{
		cell.FormatCell(value, bold);
		cell.Merge = merge;

		return cell;
	}
}
