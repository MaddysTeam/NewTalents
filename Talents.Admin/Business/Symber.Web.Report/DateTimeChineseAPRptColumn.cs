using Symber.Web.Data;
using System;
using System.Text.RegularExpressions;

namespace Symber.Web.Report
{

	public class DateTimeChineseAPRptColumn : DateTimeAPRptColumn
	{

		#region [ Constructors ]


		public DateTimeChineseAPRptColumn(DateTimeAPColumnDef columnDef, APRptDateTimeType dateTimeType)
			: base(columnDef, dateTimeType)
		{
		}


		public DateTimeChineseAPRptColumn(DateTimeAPColumnDef columnDef, APRptDateTimeType dateTimeType, DateTime minValue, DateTime maxValue)
			: base(columnDef, dateTimeType, minValue, maxValue)
		{
		}


		public DateTimeChineseAPRptColumn(DateTimeAPColumnDef columnDef, string id, string title, APRptDateTimeType dateTimeType)
			: base(columnDef, id, title, dateTimeType)
		{
		}


		public DateTimeChineseAPRptColumn(DateTimeAPColumnDef columnDef, string id, string title, APRptDateTimeType dateTimeType, DateTime minValue, DateTime maxValue)
			: base(columnDef, id, title, dateTimeType, minValue, maxValue)
		{
		}


		#endregion


		#region [ Override Implementation of APColumnEx ]


		protected override DateRange ParseDateRange(string value)
		{
			DateRange range = new DateRange();
			DateTime today = DateTime.Today;

			if (value == "昨天")
			{
				range.Start = today.AddDays(-1);
				range.End = range.Start.AddDays(1);
				return range;
			}
			else if (value == "今天")
			{
				range.Start = today;
				range.End = range.Start.AddDays(1);
				return range;
			}
			else if (value == "明天")
			{
				range.Start = today.AddDays(1);
				range.End = range.Start.AddDays(1);
				return range;
			}
			else if (value == "上周")
			{
				range.Start = today.AddDays(-(int)today.DayOfWeek - 7);
				range.End = range.Start.AddDays(8);
				return range;
			}
			else if (value == "本周")
			{
				range.Start = today.AddDays(-(int)today.DayOfWeek);
				range.End = range.Start.AddDays(8);
				return range;
			}
			else if (value == "下周")
			{
				range.Start = today.AddDays(-(int)today.DayOfWeek + 7);
				range.End = range.Start.AddDays(8);
				return range;
			}
			else if (value == "上月")
			{
				range.Start = today.AddDays(-(today.Day - 1)).AddMonths(-1);
				range.End = range.Start.AddMonths(1);
				return range;
			}
			else if (value == "本月")
			{
				range.Start = today.AddDays(-(today.Day - 1));
				range.End = range.Start.AddMonths(1);
				return range;
			}
			else if (value == "下月")
			{
				range.Start = today.AddDays(-(today.Day - 1)).AddMonths(1);
				range.End = range.Start.AddMonths(1);
				return range;
			}
			else if (value == "上季度")
			{
				range.Start = today.AddDays(-(today.Day - 1));
				range.Start.AddMonths(-((range.Start.Month + 2) % 3 + 3));
				range.End = range.Start.AddMonths(4);
				return range;
			}
			else if (value == "本季度")
			{
				range.Start = today.AddDays(-(today.Day - 1));
				range.Start.AddMonths(-((range.Start.Month + 2) % 3));
				range.End = range.Start.AddMonths(4);
				return range;
			}
			else if (value == "下季度")
			{
				range.Start = today.AddDays(-(today.Day - 1));
				range.Start.AddMonths(-((range.Start.Month + 2) % 3 - 3));
				range.End = range.Start.AddMonths(4);
				return range;
			}
			else if (value == "去年")
			{
				range.Start = new DateTime(today.Year - 1, 1, 1);
				range.End = range.Start.AddYears(1);
				return range;
			}
			else if (value == "今年")
			{
				range.Start = new DateTime(today.Year, 1, 1);
				range.End = range.Start.AddYears(1);
				return range;
			}
			else if (value == "明年")
			{
				range.Start = new DateTime(today.Year + 1, 1, 1);
				range.End = range.Start.AddYears(1);
				return range;
			}
			else if (value.StartsWith("前"))
			{
				Match match;
				int tmp;

				match = Regex.Match(value, @"^前(\d+)天$");
				if (match.Success)
				{
					if (Int32.TryParse(match.Groups[1].Value, out tmp) && tmp > 0)
					{
						range.Start = today.AddDays(-tmp);
						range.End = DateTime.Now;
						return range;
					}
					else
					{
						return null;
					}
				}
				match = Regex.Match(value, @"^前(\d+)月$");
				if (match.Success)
				{
					if (Int32.TryParse(match.Groups[1].Value, out tmp) && tmp > 0)
					{
						range.Start = today.AddDays(-(today.Day - 1)).AddMonths(-tmp);
						range.End = range.Start.AddMonths(tmp);
						return range;
					}
					else
					{
						return null;
					}
				}
				match = Regex.Match(value, @"^前(\d+)季度$");
				if (match.Success)
				{
					if (Int32.TryParse(match.Groups[1].Value, out tmp) && tmp > 0)
					{
						tmp = tmp * 3;
						range.Start = today.AddDays(-(today.Day - 1));
						range.Start.AddMonths(-((range.Start.Month + 2) % 3 + tmp));
						range.End = range.Start.AddMonths(tmp);
						return range;
					}
					else
					{
						return null;
					}
				}
				match = Regex.Match(value, @"^前(\d+)年$");
				if (match.Success)
				{
					if (Int32.TryParse(match.Groups[1].Value, out tmp) && tmp > 0)
					{
						range.Start = new DateTime(today.Year - tmp, 1, 1);
						range.End = range.Start.AddYears(tmp);
						return range;
					}
					else
					{
						return null;
					}
				}
			}
			else if (value.StartsWith("后"))
			{
				Match match;
				int tmp;

				match = Regex.Match(value, @"^后(\d+)天$");
				if (match.Success)
				{
					if (Int32.TryParse(match.Groups[1].Value, out tmp) && tmp > 0)
					{
						range.Start = today.AddDays(1);
						range.End = range.Start.AddDays(tmp);
						return range;
					}
					else
					{
						return null;
					}
				}
				match = Regex.Match(value, @"^后(\d+)月$");
				if (match.Success)
				{
					if (Int32.TryParse(match.Groups[1].Value, out tmp) && tmp > 0)
					{
						range.Start = today.AddDays(-(today.Day - 1)).AddMonths(1);
						range.End = range.Start.AddMonths(tmp);
						return range;
					}
					else
					{
						return null;
					}
				}
				match = Regex.Match(value, @"^后(\d+)季度$");
				if (match.Success)
				{
					if (Int32.TryParse(match.Groups[1].Value, out tmp) && tmp > 0)
					{
						range.Start = today.AddDays(-(today.Day - 1));
						range.Start.AddMonths(-((range.Start.Month + 2) % 3 - 3));
						range.End = range.Start.AddMonths(tmp * 3);
						return range;
					}
					else
					{
						return null;
					}
				}
				match = Regex.Match(value, @"^后(\d+)年$");
				if (match.Success)
				{
					if (Int32.TryParse(match.Groups[1].Value, out tmp) && tmp > 0)
					{
						range.Start = new DateTime(today.Year + 1, 1, 1);
						range.End = range.Start.AddYears(tmp);
						return range;
					}
					else
					{
						return null;
					}
				}
			}

			return null;
		}


		#endregion

	}

}