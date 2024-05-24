using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.Logging;
using OfficeOpenXml;
using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace RocketCalendar.Helpers
{
    public class FileIOHelper
    {
        public async Task SaveEventList_Excel(ObservableCollection<RocketEvent> events, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(filePath);

            await SaveExcelFile(events, file);
        }

        private async Task SaveExcelFile(ObservableCollection<RocketEvent> events, FileInfo file)
        {
            DeleteIfExists(file);

            using var package = new ExcelPackage(file);
            var xlWorksheet = package.Workbook.Worksheets.Add("Rocket Events List");
            var range = xlWorksheet.Cells["A1"].LoadFromCollection(events, true, OfficeOpenXml.Table.TableStyles.Medium1);

            var max = xlWorksheet.Column(1).ColumnMax;
            
            range.AutoFitColumns();

            await package.SaveAsync();
        }

        private static void DeleteIfExists(FileInfo file)
        {
            if(file.Exists)
            {
                file.Delete();
            }
        }

        public ObservableCollection<RocketEvent> LoadEventList_Excel(string filePath)
        {
            ObservableCollection<RocketEvent> events = new ObservableCollection<RocketEvent>();

            var file = new FileInfo(filePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                int row = 2;
                int col = 1;

                while (string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Value?.ToString()) == false)
                {
                    RocketEvent e = new RocketEvent(new RocketDate(1, 1, 1), "", "", false, 1);

                    e.EventName = worksheet.Cells[row, col].Value.ToString();
                    e.EventDescription = worksheet.Cells[row, col + 2].Value.ToString();
                    e.IsPrivate = bool.Parse(worksheet.Cells[row, col + 3].Value.ToString());
                    e.ColorIndex = int.Parse(worksheet.Cells[row, col + 4].Value.ToString());
                    e.MonthRepeatInterval = int.Parse(worksheet.Cells[row, col + 5].Value.ToString());
                    e.YearRepeatInterval = int.Parse(worksheet.Cells[row, col + 6].Value.ToString());
                    e.EventDate.DateDay = int.Parse(worksheet.Cells[row, col + 7].Value.ToString());
                    e.EventDate.DateMonth = int.Parse(worksheet.Cells[row, col + 8].Value.ToString());
                    e.EventDate.DateYear = int.Parse(worksheet.Cells[row, col + 9].Value.ToString());
                    events.Add(e);
                    row++;
                }
            }
            return events;
        }

        //Currently Unimplemented
        public async Task<ObservableCollection<RocketEvent>> LoadEventList_Excel_async(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(filePath);

            ObservableCollection<RocketEvent> events = new ObservableCollection<RocketEvent>();
            List<RocketEvent> eventsFromExcel = await LoadFromExcelFile(file);

            foreach (RocketEvent e in eventsFromExcel)
            {
                events.Add(e);
            }

            return events;
        }

        private static async Task<List<RocketEvent>> LoadFromExcelFile(FileInfo file)
        {
            List<RocketEvent> events = new();


            using var package = new ExcelPackage(file);

            await package.LoadAsync(file);

            var xlWorksheet = package.Workbook.Worksheets[0];

            //ignoring headers
            int row = 2; 
            int col = 1;

            while (string.IsNullOrWhiteSpace(xlWorksheet.Cells[row,col].Value?.ToString()) == false)
            {
                RocketEvent e = new RocketEvent(new RocketDate(1, 1, 1), "", "", false, 1);

                e.EventName = xlWorksheet.Cells[row, col].Value.ToString();
                e.EventDescription = xlWorksheet.Cells[row, col + 2].Value.ToString();
                e.IsPrivate = bool.Parse(xlWorksheet.Cells[row, col + 3].Value.ToString());
                e.ColorIndex = int.Parse(xlWorksheet.Cells[row, col + 4].Value.ToString());
                e.MonthRepeatInterval = int.Parse(xlWorksheet.Cells[row, col + 5].Value.ToString());
                e.YearRepeatInterval = int.Parse(xlWorksheet.Cells[row, col + 6].Value.ToString());
                e.EventDate.DateDay = int.Parse(xlWorksheet.Cells[row, col + 7].Value.ToString());
                e.EventDate.DateMonth = int.Parse(xlWorksheet.Cells[row, col + 8].Value.ToString());
                e.EventDate.DateYear = int.Parse(xlWorksheet.Cells[row, col + 9].Value.ToString());
                events.Add(e);
                row++;
            }
            return events;
        }

        public void SaveEventList_XML(ObservableCollection<RocketEvent> events, string filePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("RocketXML");
            XmlAttribute rocketFileTypeAttribute = doc.CreateAttribute("RocketFileType");
            rocketFileTypeAttribute.Value = "RocketCalendarEventList";
            rootNode.Attributes.Append(rocketFileTypeAttribute);

            doc.AppendChild(rootNode);

            XmlNode eventNode;
            XmlAttribute eventNameAttribute;
            XmlAttribute eventDateDayAttribute;
            XmlAttribute eventDateMonthAttribute;
            XmlAttribute eventDateYearAttribute;
            XmlAttribute eventIsPrivateAttribute;
            XmlAttribute eventColorIndexAttribute;
            XmlAttribute eventMonthRepeatIntervalAttribute;
            XmlAttribute eventYearRepeatIntervalAttribute;

            foreach (RocketEvent e in events)
            {
                eventNode = doc.CreateElement("Event");
                eventNameAttribute = doc.CreateAttribute("Name");
                eventDateDayAttribute = doc.CreateAttribute("DateDay");
                eventDateMonthAttribute = doc.CreateAttribute("DateMonth");
                eventDateYearAttribute = doc.CreateAttribute("DateYear");
                eventIsPrivateAttribute = doc.CreateAttribute("IsPrivate");
                eventColorIndexAttribute = doc.CreateAttribute("ColorIndex");
                eventMonthRepeatIntervalAttribute = doc.CreateAttribute("MonthRepeatInterval");
                eventYearRepeatIntervalAttribute = doc.CreateAttribute("YearRepeatInterval");

                eventNameAttribute.Value = e.EventName;
                eventDateDayAttribute.Value = e.EventDate.DateDay.ToString();
                eventDateMonthAttribute.Value = e.EventDate.DateMonth.ToString();
                eventDateYearAttribute.Value = e.EventDate.DateYear.ToString();
                eventIsPrivateAttribute.Value = e.IsPrivate.ToString();
                eventColorIndexAttribute.Value = e.ColorIndex.ToString();
                eventMonthRepeatIntervalAttribute.Value = e.MonthRepeatInterval.ToString();
                eventYearRepeatIntervalAttribute.Value = e.YearRepeatInterval.ToString();

                eventNode.Attributes.Append(eventNameAttribute);
                eventNode.Attributes.Append(eventDateDayAttribute);
                eventNode.Attributes.Append(eventDateMonthAttribute);
                eventNode.Attributes.Append(eventDateYearAttribute);
                eventNode.Attributes.Append(eventIsPrivateAttribute);
                eventNode.Attributes.Append(eventColorIndexAttribute);
                eventNode.Attributes.Append(eventMonthRepeatIntervalAttribute);
                eventNode.Attributes.Append(eventYearRepeatIntervalAttribute);

                eventNode.InnerText = e.EventDescription + "\n";
                rootNode.AppendChild(eventNode);
            }    

            XmlWriterSettings xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(filePath, xmlSettings))
            {
                doc.Save(writer);
            }
        }

        public ObservableCollection<RocketEvent> LoadEventList_XML(string filePath)
        {
            ObservableCollection<RocketEvent> events = new ObservableCollection<RocketEvent>();

            if(File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlElement root = doc.DocumentElement;

                if(root.Name == "RocketXML")
                {
                    XmlAttribute rocketFileTypeAttribute = root.Attributes["RocketFileType"];
                    if(rocketFileTypeAttribute.Value == "RocketCalendarEventList")
                    {
                        XmlNodeList nodes = root.SelectNodes("Event");

                        XmlAttribute eventNameAttribute;
                        XmlAttribute eventDateDayAttribute;
                        XmlAttribute eventDateMonthAttribute;
                        XmlAttribute eventDateYearAttribute;
                        XmlAttribute eventIsPrivateAttribute;
                        XmlAttribute eventColorIndexAttribute;
                        XmlAttribute eventMonthRepeatIntervalAttribute;
                        XmlAttribute eventYearRepeatIntervalAttribute;

                        RocketEvent temporaryEvent;

                        foreach (XmlNode node in nodes)
                        {
                            //eventNode = node.Attributes["Event"];
                            eventNameAttribute = node.Attributes["Name"];
                            eventDateDayAttribute = node.Attributes["DateDay"];
                            eventDateMonthAttribute = node.Attributes["DateMonth"];
                            eventDateYearAttribute = node.Attributes["DateYear"];
                            eventIsPrivateAttribute = node.Attributes["IsPrivate"];
                            eventColorIndexAttribute = node.Attributes["ColorIndex"];
                            eventMonthRepeatIntervalAttribute = node.Attributes["MonthRepeatInterval"];
                            eventYearRepeatIntervalAttribute = node.Attributes["YearRepeatInterval"];

                            temporaryEvent = GetTemporaryEvent();

                            if (eventNameAttribute != null) { temporaryEvent.EventName = eventNameAttribute.Value; }
                            if (eventDateDayAttribute != null) { temporaryEvent.EventDate.DateDay = Int32.Parse(eventDateDayAttribute.Value); }
                            if (eventDateMonthAttribute != null) { temporaryEvent.EventDate.DateMonth = Int32.Parse(eventDateMonthAttribute.Value); }
                            if (eventDateYearAttribute != null) { temporaryEvent.EventDate.DateYear = Int32.Parse(eventDateYearAttribute.Value); }
                            if (eventIsPrivateAttribute != null) { temporaryEvent.IsPrivate = XmlConvert.ToBoolean(eventIsPrivateAttribute.Value.ToLower()); }
                            if (eventColorIndexAttribute != null) { temporaryEvent.ColorIndex = Int32.Parse(eventColorIndexAttribute.Value); }
                            if (eventMonthRepeatIntervalAttribute != null) { temporaryEvent.MonthRepeatInterval = Int32.Parse(eventMonthRepeatIntervalAttribute.Value); }
                            if (eventYearRepeatIntervalAttribute != null) { temporaryEvent.YearRepeatInterval = Int32.Parse(eventYearRepeatIntervalAttribute.Value); }

                            temporaryEvent.EventDescription = node.InnerText;
                            
                            events.Add(temporaryEvent);
                        }
                    }
                    else
                    {
                        //else not a RocketEventList file
                        return null;
                    }
                    
                }
                else
                {
                    //else not a RocketXML file
                    return null;
                }
            }
            else
            {
                //file can't be found
                return null;
            }
            return events;
        }

        public void SaveCalendar_XML(RocketCalendarModel calendar, string filePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement("RocketXML");
            XmlAttribute rocketFileTypeAttribute = doc.CreateAttribute("RocketFileType");
            rocketFileTypeAttribute.Value = "RocketCalendar";
            rootNode.Attributes.Append(rocketFileTypeAttribute);

            doc.AppendChild(rootNode);

            XmlNode calendarNode;
            XmlAttribute calendarNameAttribute;
            XmlAttribute calendarBaseDateDayAttribute;
            XmlAttribute calendarBaseDateMonthAttribute;
            XmlAttribute calendarBaseDateYearAttribute;
            XmlAttribute calendarBaseDateDayIndexAttribute;
            XmlAttribute calendarCurrentYearAttribute;
            XmlAttribute calendarCurrentMonthAttribute;

            calendarNode = doc.CreateElement("Calendar");
            calendarNameAttribute = doc.CreateAttribute("Name");
            calendarBaseDateDayAttribute = doc.CreateAttribute("BaseDateDay");
            calendarBaseDateMonthAttribute = doc.CreateAttribute("BaseDateMonth");
            calendarBaseDateYearAttribute = doc.CreateAttribute("BaseDateYear");
            calendarBaseDateDayIndexAttribute = doc.CreateAttribute("BaseDateIndex");
            calendarCurrentYearAttribute = doc.CreateAttribute("CurrentYear");
            calendarCurrentMonthAttribute = doc.CreateAttribute("CurrentMonth");

            calendarNameAttribute.Value = calendar.CalendarName;
            calendarBaseDateDayAttribute.Value = calendar.BaseDate.DateDay.ToString();
            calendarBaseDateMonthAttribute.Value = calendar.BaseDate.DateMonth.ToString();
            calendarBaseDateYearAttribute.Value = calendar.BaseDate.DateYear.ToString();
            calendarBaseDateDayIndexAttribute.Value = calendar.BaseDate.DayIndex.ToString();
            calendarCurrentYearAttribute.Value = calendar.CurrentYear.ToString();
            calendarCurrentMonthAttribute.Value = calendar.CurrentMonth.ToString();

            calendarNode.Attributes.Append(calendarNameAttribute);
            calendarNode.Attributes.Append(calendarBaseDateDayAttribute);
            calendarNode.Attributes.Append(calendarBaseDateMonthAttribute);
            calendarNode.Attributes.Append(calendarBaseDateYearAttribute);
            calendarNode.Attributes.Append(calendarBaseDateDayIndexAttribute);
            calendarNode.Attributes.Append(calendarCurrentYearAttribute);
            calendarNode.Attributes.Append(calendarCurrentMonthAttribute);


            //Collections:

            XmlNode monthsNode;
            monthsNode = doc.CreateElement("Months");
            XmlNode monthNode;
            XmlAttribute monthNameAttribute;
            XmlAttribute monthNumOfDaysAttribute;
            XmlAttribute monthLeapYearIntervalAttribute;

            foreach (RocketMonth m in calendar.MonthCollection)
            {
                monthNode = doc.CreateElement("Month");
                monthNameAttribute = doc.CreateAttribute("Name");
                monthNumOfDaysAttribute = doc.CreateAttribute("NumOfDays");
                monthLeapYearIntervalAttribute = doc.CreateAttribute("LeapYearInterval");

                monthNameAttribute.Value = m.Name.ToString();
                monthNumOfDaysAttribute.Value = m.NumOfDays.ToString();

                monthNode.Attributes.Append(monthNameAttribute);
                monthNode.Attributes.Append(monthNumOfDaysAttribute);

                if (m.LeapYearInterval != null)
                {
                    monthLeapYearIntervalAttribute.Value = m.LeapYearInterval.ToString();
                    monthNode.Attributes.Append(monthLeapYearIntervalAttribute);
                }

                monthsNode.AppendChild(monthNode);
            }

            calendarNode.AppendChild(monthsNode);


            XmlNode dayNamesNode;
            dayNamesNode = doc.CreateElement("DayNames");
            XmlNode dayNameNode;
            XmlAttribute dayNameAttribute;

            foreach (string n in calendar.DayNameCollection)
            {
                dayNameNode = doc.CreateElement("DayName");
                dayNameAttribute = doc.CreateAttribute("Name");

                dayNameAttribute.Value = n;

                dayNameNode.Attributes.Append(dayNameAttribute);

                dayNamesNode.AppendChild(dayNameNode);
            }

            calendarNode.AppendChild(dayNamesNode);


            XmlNode eventsNode;
            eventsNode = doc.CreateElement("Events");
            XmlNode eventNode;
            XmlAttribute eventNameAttribute;
            XmlAttribute eventDateDayAttribute;
            XmlAttribute eventDateMonthAttribute;
            XmlAttribute eventDateYearAttribute;
            XmlAttribute eventIsPrivateAttribute;
            XmlAttribute eventColorIndexAttribute;
            XmlAttribute eventMonthRepeatIntervalAttribute;
            XmlAttribute eventYearRepeatIntervalAttribute;

            foreach (RocketEvent e in calendar.EventCollection)
            {
                eventNode = doc.CreateElement("Event");
                eventNameAttribute = doc.CreateAttribute("Name");
                eventDateDayAttribute = doc.CreateAttribute("DateDay");
                eventDateMonthAttribute = doc.CreateAttribute("DateMonth");
                eventDateYearAttribute = doc.CreateAttribute("DateYear");
                eventIsPrivateAttribute = doc.CreateAttribute("IsPrivate");
                eventColorIndexAttribute = doc.CreateAttribute("ColorIndex");
                eventMonthRepeatIntervalAttribute = doc.CreateAttribute("MonthRepeatInterval");
                eventYearRepeatIntervalAttribute = doc.CreateAttribute("YearRepeatInterval");

                eventNameAttribute.Value = e.EventName;
                eventDateDayAttribute.Value = e.EventDate.DateDay.ToString();
                eventDateMonthAttribute.Value = e.EventDate.DateMonth.ToString();
                eventDateYearAttribute.Value = e.EventDate.DateYear.ToString();
                eventIsPrivateAttribute.Value = e.IsPrivate.ToString();
                eventColorIndexAttribute.Value = e.ColorIndex.ToString();
                eventMonthRepeatIntervalAttribute.Value = e.MonthRepeatInterval.ToString();
                eventYearRepeatIntervalAttribute.Value = e.YearRepeatInterval.ToString();

                eventNode.Attributes.Append(eventNameAttribute);
                eventNode.Attributes.Append(eventDateDayAttribute);
                eventNode.Attributes.Append(eventDateMonthAttribute);
                eventNode.Attributes.Append(eventDateYearAttribute);
                eventNode.Attributes.Append(eventIsPrivateAttribute);
                eventNode.Attributes.Append(eventColorIndexAttribute);
                eventNode.Attributes.Append(eventMonthRepeatIntervalAttribute);
                eventNode.Attributes.Append(eventYearRepeatIntervalAttribute);

                eventNode.InnerText = e.EventDescription + "\n";
                eventsNode.AppendChild(eventNode);
            }
            calendarNode.AppendChild(eventsNode);


            rootNode.AppendChild(calendarNode);

            XmlWriterSettings xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(filePath, xmlSettings))
            {
                doc.Save(writer);
            }
        }
 
        public RocketCalendarModel LoadCalendar_XML(string filePath)
        {
            RocketCalendarModel temporaryCalendar = GetTemporaryCalendar();

            if (File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlElement root = doc.DocumentElement;

                if (root.Name == "RocketXML")
                {
                    XmlAttribute rocketFileTypeAttribute = root.Attributes["RocketFileType"];
                    if (rocketFileTypeAttribute.Value == "RocketCalendar")
                    {
                       
                        XmlNodeList nodes = root.SelectNodes("Calendar");

                        XmlNode calendarNode = nodes[0];
                        XmlAttribute calendarNameAttribute;
                        XmlAttribute calendarBaseDateDayAttribute;
                        XmlAttribute calendarBaseDateMonthAttribute;
                        XmlAttribute calendarBaseDateYearAttribute;
                        XmlAttribute calendarBaseDateDayIndexAttribute;
                        XmlAttribute calendarCurrentYearAttribute;
                        XmlAttribute calendarCurrentMonthAttribute;

                        calendarNameAttribute = calendarNode.Attributes["Name"];
                        calendarBaseDateDayAttribute = calendarNode.Attributes["BaseDateDay"];
                        calendarBaseDateMonthAttribute = calendarNode.Attributes["BaseDateMonth"];
                        calendarBaseDateYearAttribute = calendarNode.Attributes["BaseDateYear"];
                        calendarBaseDateDayIndexAttribute = calendarNode.Attributes["BaseDateIndex"];
                        calendarCurrentYearAttribute = calendarNode.Attributes["CurrentYear"];
                        calendarCurrentMonthAttribute = calendarNode.Attributes["CurrentMonth"];

                        if (calendarNameAttribute != null) { temporaryCalendar.CalendarName =  calendarNameAttribute.Value; }
                        if (calendarBaseDateDayAttribute != null) { temporaryCalendar.BaseDate.DateDay = Int32.Parse(calendarBaseDateDayAttribute.Value); }
                        if (calendarBaseDateMonthAttribute != null) { temporaryCalendar.BaseDate.DateMonth = Int32.Parse(calendarBaseDateMonthAttribute.Value); }
                        if (calendarBaseDateYearAttribute != null) { temporaryCalendar.BaseDate.DateYear = Int32.Parse(calendarBaseDateYearAttribute.Value); }
                        if (calendarBaseDateDayIndexAttribute != null) { temporaryCalendar.BaseDate.DayIndex = Int32.Parse(calendarBaseDateDayIndexAttribute.Value); }
                        if (calendarCurrentYearAttribute != null) { temporaryCalendar.CurrentYear = Int32.Parse(calendarCurrentYearAttribute.Value); }
                        if (calendarCurrentMonthAttribute != null) { temporaryCalendar.CurrentMonth = Int32.Parse(calendarCurrentMonthAttribute.Value); }

                        //Collections
                        const int MIN_NUM_OF_MONTH_ATTRIBUTES = 2;

                        XmlNode MonthsNode = calendarNode.SelectSingleNode("Months");
                        XmlNodeList months = MonthsNode.SelectNodes("Month");
                        foreach (XmlNode monthNode in months)
                        {
                            RocketMonth temporaryMonth = GetTemporaryMonth();
                            XmlAttribute monthNameAttribute;
                            XmlAttribute monthNumOfDaysAttribute;
                            XmlAttribute monthLeapYearIntervalAttribute;

                            monthNameAttribute = monthNode.Attributes["Name"];
                            monthNumOfDaysAttribute = monthNode.Attributes["NumOfDays"];
                            
                            if (monthNameAttribute != null) { temporaryMonth.Name = monthNameAttribute.Value; }
                            if (monthNumOfDaysAttribute != null) { temporaryMonth.NumOfDays = Int32.Parse(monthNumOfDaysAttribute.Value); }

                            if (monthNode.Attributes.Count > MIN_NUM_OF_MONTH_ATTRIBUTES)
                            {
                                monthLeapYearIntervalAttribute = monthNode.Attributes["LeapYearInterval"];
                                if (monthLeapYearIntervalAttribute != null) { temporaryMonth.LeapYearInterval = Int32.Parse(monthLeapYearIntervalAttribute.Value); }
                            }
                            temporaryCalendar.MonthCollection.Add(temporaryMonth);
                        }

                        XmlNode DayNamesNode = calendarNode.SelectSingleNode("DayNames");
                        XmlNodeList dayNames = DayNamesNode.SelectNodes("DayName");
                        foreach (XmlNode dayNameNode in dayNames)
                        {
                            XmlAttribute dayNameAttribute;
                            dayNameAttribute = dayNameNode.Attributes["Name"];
                            if (dayNameAttribute != null)
                            {
                                temporaryCalendar.DayNameCollection.Add(dayNameAttribute.Value);
                            }
                        }

                        XmlNode EventsNode = calendarNode.SelectSingleNode("Events");
                        XmlNodeList events = EventsNode.SelectNodes("Event");
                        foreach (XmlNode eventNode in events)
                        {
                            RocketEvent temporaryEvent;

                            XmlAttribute eventNameAttribute;
                            XmlAttribute eventDateDayAttribute;
                            XmlAttribute eventDateMonthAttribute;
                            XmlAttribute eventDateYearAttribute;
                            XmlAttribute eventIsPrivateAttribute;
                            XmlAttribute eventColorIndexAttribute;
                            XmlAttribute eventMonthRepeatIntervalAttribute;
                            XmlAttribute eventYearRepeatIntervalAttribute;

                            eventNameAttribute = eventNode.Attributes["Name"];
                            eventDateDayAttribute = eventNode.Attributes["DateDay"];
                            eventDateMonthAttribute = eventNode.Attributes["DateMonth"];
                            eventDateYearAttribute = eventNode.Attributes["DateYear"];
                            eventIsPrivateAttribute = eventNode.Attributes["IsPrivate"];
                            eventColorIndexAttribute = eventNode.Attributes["ColorIndex"];
                            eventMonthRepeatIntervalAttribute = eventNode.Attributes["MonthRepeatInterval"];
                            eventYearRepeatIntervalAttribute = eventNode.Attributes["YearRepeatInterval"];

                            temporaryEvent = GetTemporaryEvent();

                            if (eventNameAttribute != null) { temporaryEvent.EventName = eventNameAttribute.Value; }
                            if (eventDateDayAttribute != null) { temporaryEvent.EventDate.DateDay = Int32.Parse(eventDateDayAttribute.Value); }
                            if (eventDateMonthAttribute != null) { temporaryEvent.EventDate.DateMonth = Int32.Parse(eventDateMonthAttribute.Value); }
                            if (eventDateYearAttribute != null) { temporaryEvent.EventDate.DateYear = Int32.Parse(eventDateYearAttribute.Value); }
                            if (eventIsPrivateAttribute != null) { temporaryEvent.IsPrivate = XmlConvert.ToBoolean(eventIsPrivateAttribute.Value.ToLower()); }
                            if (eventColorIndexAttribute != null) { temporaryEvent.ColorIndex = Int32.Parse(eventColorIndexAttribute.Value); }
                            if (eventMonthRepeatIntervalAttribute != null) { temporaryEvent.MonthRepeatInterval = Int32.Parse(eventMonthRepeatIntervalAttribute.Value); }
                            if (eventYearRepeatIntervalAttribute != null) { temporaryEvent.YearRepeatInterval = Int32.Parse(eventYearRepeatIntervalAttribute.Value); }

                            temporaryEvent.EventDescription = eventNode.InnerText;

                            temporaryCalendar.EventCollection.Add(temporaryEvent);
                        }
                    }
                    else
                    {
                        //wrong type of Rocket File
                        return null;
                    }
                }
                else
                {
                    //not a Rocket File
                    return null;
                }
                
            }
            else
            {
                //can't find file
                return null;
            }
            return temporaryCalendar;
        }
         
        private RocketEvent GetTemporaryEvent()
        {
            RocketEvent e = new RocketEvent(new RocketDate(1, 1, 1), "", "", false, 1);
            return e;
        }

        private RocketCalendarModel GetTemporaryCalendar()
        {
            RocketCalendarModel c = new RocketCalendarModel(
                "",
                new RocketDate(1, 1, 1),
                new ObservableCollection<RocketMonth>(),
                new ObservableCollection<string>(),
                new ObservableCollection<RocketEvent>(),
                1,
                1
                );
            return c;
        }

        private RocketMonth GetTemporaryMonth()
        {
            return new RocketMonth("", 1);
        }
    }
}
