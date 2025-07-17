using System;

namespace StateMachine
{
    public class DoubleEntryTable<RT, CT, VT>
    {
        private readonly RT[] _rows;
        private readonly CT[] _columns;
        private readonly VT[,] _values;
        
        public DoubleEntryTable(RT[] rows, CT[] columns)
        {
            _rows = rows;
            _columns = columns;
            _values = new VT[rows.Length, columns.Length];
        }

        public VT this[RT row, CT column]
        {
            get
            {
                var i = Array.IndexOf(_rows, row);
                var j = Array.IndexOf(_columns, column);

                return _values[i, j];
            }

            set
            {
                var i = Array.IndexOf(_rows, row);
                var j = Array.IndexOf(_columns, column);

                _values[i, j] = value;
            }
        }
    }
}
