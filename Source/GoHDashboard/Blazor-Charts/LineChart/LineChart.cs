using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAssemblyMan.SVGRender;

namespace WebAssemblyMan
{
    public class LineChart : ComponentBase
    {

        /// <summary>
        /// Input specified as "[x1,x2, ...],[y1,y2, ...],...".
        /// </summary>
        [Parameter]
        public string InputSeries { get; set; }

        [Parameter]
        public string InputLabels { get; set; }
        [Parameter]
        public string XAxisLabels { get; set; }

        //1. xaxis text labels dx dy
        //2. expose gridyunits
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var seq = 0;
            builder.OpenElement(seq, "figure");
            builder.AddAttribute(++seq, "class", "line-chart");
            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "main");

            var inputLabelsArr = InputLabels.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var xAxisLabelsArr = XAxisLabels.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var inputLines = InputSeries.Trim('[', ']').Split("],[");
            var inputNumbers = inputLines.Select(l => l.Split(',').Select(double.Parse).ToArray()).ToArray();
            
            double maxY=0.0;
            var numValues = 0;
            foreach (var data in inputNumbers)
            {
	            maxY = Math.Max(data.Max(), maxY);
	            numValues = Math.Max(data.Length, numValues);
            }

            double boundHeight = 150.0;
            double boundWidth = 150.0;

            SVG svg = new SVG() { { "width", "100%" }, { "height", "100%" }, { "viewBox", "0 0 150 150" } };
            Rectangle rect = new Rectangle() { { "class", "background-rect" }};
            svg.AddItems(rect);

            /*
            int numHorizontalLines = 10;
            int numVerticalLines = 10;
            */

            double gridYUnits = 10;
            double gridXUnits = 10; //not required

            int numXLabels = xAxisLabelsArr.Length;

            //1. Determine number of input values in xaxis and use it for numVerticalLines
            int numVerticalLines = numValues;
            
            //2. Detemine max value in yaxis and then use it calculate numHorizontalLines
            int numHorizontalLines = ((int) (maxY / gridYUnits))+1;

            double verticalStartSpace = 25.0;
            double horizontalStartSpace = 25.0;
            double verticalEndSpace = 25.0;
            double horizontalEndSpace = 25.0;

            double verticalSpace = (boundHeight- verticalStartSpace-verticalEndSpace) / (numHorizontalLines);
            double horizontalSpace = (boundWidth - horizontalStartSpace-horizontalEndSpace) / (numVerticalLines);

            double totalGridWidth = ((double)(numVerticalLines-1)) * horizontalSpace;
            double totalGridHeight = ((double)(numHorizontalLines-1)) * verticalSpace;
            System.Diagnostics.Debug.WriteLine("TotalGridHeight:" + totalGridHeight+":"+ verticalSpace);

            //Horizontal Lines
            double y = verticalStartSpace;
            double startGridY = 0;
            for (int counter=0;counter<=numHorizontalLines;counter++)
            {
                Path path = new Path() { { "class", "horizontal-grid-lines" }, { "d", "M "+horizontalStartSpace.ToString()+" "+(boundHeight - y).ToString() + " L "+(boundWidth-horizontalEndSpace).ToString()+" "+(boundHeight - y).ToString() } };
                Text label = new Text() { { "class", "y-axis-labels" }, { "x", (horizontalStartSpace-2).ToString() }, { "y", (boundHeight - y).ToString() }, { "content", (startGridY).ToString() } };

                svg.AddItems(path,label);
                //System.Diagnostics.Debug.WriteLine("Y:" + y);
                y = y + verticalSpace;
                startGridY = startGridY + gridYUnits;
                //note : gridYUnits is the value the user see
                //verticalSpace is the internal/actual value used to represent gridYUnits on the chart.
            }

            //Chart Line
            double gridx=0, gridy = 0;
            gridx = horizontalStartSpace;
            gridy = boundHeight - verticalStartSpace;
            int colorcounter = 0;
            foreach (var data in inputNumbers)
            {
                string chartLine = "";
                double gridValueX = 0;
                double gridValueY = 0;
                bool firstTime = true;

                double[] intAry=new double[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
	                intAry[i]=data[i];
                }


                foreach (int i in intAry)
                {
                    if (firstTime)
                    {
                        chartLine = chartLine + "M ";
                        firstTime = false;
                        gridValueX = horizontalStartSpace;
                        gridValueY = verticalStartSpace;
                        double gridValue = ((double)i) * verticalSpace / gridYUnits;
                        gridValueY = boundHeight - (gridValueY + gridValue);
                        chartLine = chartLine + gridValueX.ToString() + " " + gridValueY.ToString();
                    }
                    else
                    {
                        chartLine = chartLine + " L ";
                        gridValueX = gridValueX + horizontalSpace;
                        gridValueY = verticalStartSpace;
                        //if 5 verticalSapce represents 10 gridYUnits
                        //when you have 10 it becomes 10*5/10=5
                        double gridValue = ((double)i) * verticalSpace / gridYUnits;
                        gridValueY = boundHeight - (gridValueY + gridValue);                        
                        chartLine = chartLine + gridValueX.ToString() + " " + gridValueY.ToString();
                    }
                }
                Path linepath = new Path() { { "class", "line-"+(colorcounter+1).ToString() },{ "d", chartLine } };
                colorcounter++;
                svg.AddItems(linepath);

            }

            //Vertical Lines            
            double x = horizontalStartSpace;
            double startGridX = 0;
            int xLabelsCounter=0;

            for (int counter = 0; counter <= numVerticalLines; counter++)
            {

                Path path = new Path() { { "class", "vertical-grid-lines" }, { "d", "M " + x.ToString() +" "+ (boundHeight-verticalStartSpace).ToString() + " L "+ x.ToString() + " " +(verticalEndSpace).ToString() } };

                string xLabels="";
                if (xLabelsCounter<numXLabels)
                    xLabels=xAxisLabelsArr[xLabelsCounter++];

                Text label = new Text() { { "class", "x-axis-labels" }, {"transform", "translate("+x.ToString()+","+(boundHeight - verticalStartSpace + 5).ToString()+")" },{"dx","+1em"},{"dy","0.30em"}, { "content", xLabels } };

                //not required. just need number of grid lines
                startGridX = startGridX + gridXUnits;

                svg.AddItems(path,label);
                x = x + horizontalSpace;
            }
            
            BlazorRenderer blazorRenderer = new BlazorRenderer();
            blazorRenderer.Draw(seq, builder, svg);
            
            builder.OpenElement(++seq, "figcaption");
            builder.AddAttribute(++seq, "class", "key");
            builder.OpenElement(++seq, "ul");
            builder.AddAttribute(++seq, "class", "key-list");

            colorcounter = 0;
            foreach (var iData in inputLines)
            {
                builder.OpenElement(++seq, "li");
                builder.OpenElement(++seq, "span");
                builder.AddAttribute(++seq, "class", "legend-"+(colorcounter+1).ToString());

                builder.CloseElement();

                string label="";
                if (colorcounter<inputLabelsArr.Length)
                    label=inputLabelsArr[colorcounter];

                builder.AddContent(++seq, label);
                builder.CloseElement();
                colorcounter++;
            }

            builder.CloseElement();
            builder.CloseElement();
            

            builder.CloseElement();
            builder.CloseElement();



        }
    }
}
