using Microsoft.Extensions.Logging;
using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace RocketCalendar.Helpers
{
    public class FileIOHelper
    {
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

        //to do: needs testing
        public ObservableCollection<RocketEvent> LoadEventList_XML(string filePath)
        {
            ObservableCollection<RocketEvent> events = new ObservableCollection<RocketEvent>();

            if(File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                XmlElement root = doc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("RocketXML");
                XmlAttribute rocketFileTypeAttribute = root.Attributes["RocketFileType"];

                XmlAttribute eventNameAttribute;
                XmlAttribute eventDateDayAttribute;
                XmlAttribute eventDateMonthAttribute;
                XmlAttribute eventDateYearAttribute;
                XmlAttribute eventIsPrivateAttribute;
                XmlAttribute eventColorIndexAttribute;
                XmlAttribute eventMonthRepeatIntervalAttribute;
                XmlAttribute eventYearRepeatIntervalAttribute;

                if (rocketFileTypeAttribute.Value == "RocketCalendarEventList")
                {
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

                        events.Add(temporaryEvent);
                    }
                }
                else
                {
                    //file can't be found
                }
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

        /*
         public RocketCalendarModel LoadCalendar_XML(string filePath)
        {
            RocketCalendarModel calendar = new RocketCalendarModel();

            return calendar;
        }
         */

        private RocketEvent GetTemporaryEvent()
        {
            RocketEvent e = new RocketEvent(new RocketDate(1, 1, 1), "", "", false, 1);
            return e;
        }
    }
}
