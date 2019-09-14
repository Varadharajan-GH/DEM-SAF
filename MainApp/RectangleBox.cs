using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainApp
{
    public class RectangleBox
    {
        public Rectangle rectBox;
        private Point _startPoint;
        private Point _endPoint;
        public int width;
        public int height;

        public RectangleBox()
        {
            rectBox =Rectangle.Empty ;
            width = 0;
            height = 0;
        }

        public Point StartPoint
        {
            get { return _startPoint; }
            set {                
                _startPoint.X = value.X;
                _startPoint.Y = value.Y;
                if (_endPoint.IsEmpty)
                {
                    _endPoint = _startPoint;
                }
                width = Math.Abs(_startPoint.X - _endPoint.X);
                height = Math.Abs(_startPoint.Y - _endPoint.Y);
                rectBox = new Rectangle(Math.Min(_startPoint.X, _endPoint.X), Math.Min(_startPoint.Y, _endPoint.Y), width, height);
            }
        }

        public Point EndPoint
        {
            get { return _endPoint; }
            set
            {
                _endPoint.X = value.X;
                _endPoint.Y = value.Y;
                if (_startPoint.IsEmpty)
                {
                    _startPoint = _endPoint;
                }
                width = Math.Abs(_startPoint.X - _endPoint.X);
                height = Math.Abs(_startPoint.Y - _endPoint.Y);
                rectBox = new Rectangle(Math.Min(_startPoint.X, _endPoint.X), Math.Min(_startPoint.Y, _endPoint.Y), width, height);
            }
        }
    }
}
