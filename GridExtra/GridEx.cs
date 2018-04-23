using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using System.Reactive.Linq;


using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Diagnostics;

namespace GridExtra
{

    using LayoutUpdateEventHandler = EventHandler;

    public class GridEx
    {
        static GridEx()
        {
            AutoFillChildrenProperty.Changed.Subscribe(GridEx.OnAutoFillChildrenChanged);
            TemplateAreaProperty.Changed.Subscribe(GridEx.OnTemplateAreaChanged);
            AreaNameProperty.Changed.Subscribe(GridEx.OnAreaNameChanged);
            AreaProperty.Changed.Subscribe(GridEx.OnAreaChanged);
        }

        public static Orientation GetAutoFillOrientation(AvaloniaObject obj)
        {
            return obj.GetValue(AutoFillOrientationProperty);
        }
        public static void SetAutoFillOrientation(AvaloniaObject obj, Orientation value)
        {
            obj.SetValue(AutoFillOrientationProperty, value);
        }
        // Using a AvaloniaProperty as the backing store for AutoFillOrientation.  This enables animation, styling, binding, etc...
        public static readonly AvaloniaProperty<Orientation> AutoFillOrientationProperty =
            AvaloniaProperty.RegisterAttached<GridEx, Control, Orientation>("AutoFillOrientation", Orientation.Horizontal);

        public static bool GetAutoFillChildren(AvaloniaObject obj)
        {
            return obj.GetValue(AutoFillChildrenProperty);
        }
        public static void SetAutoFillChildren(AvaloniaObject obj, bool value)
        {
            obj.SetValue(AutoFillChildrenProperty, value);
        }

        // Using a AvaloniaProperty as the backing store for AutoFillChildren.  This enables animation, styling, binding, etc...
        public static readonly AvaloniaProperty<bool> AutoFillChildrenProperty =
            AvaloniaProperty.RegisterAttached<GridEx, Control, bool>("AutoFillChildren", false);

        private static void OnAutoFillChildrenChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var grid = e.Sender as Grid;
            var isEnabled = (bool)e.NewValue;
            if (grid == null) { return; }

            if (isEnabled)
            {
                var layoutUpdateCallback = CreateLayoutUpdateHandler(grid);
                // イベントの登録
                grid.LayoutUpdated += layoutUpdateCallback;
                SetLayoutUpdatedCallback(grid, layoutUpdateCallback);

                // AutoFill処理を行う
                AutoFill(grid);
            }
            else
            {
                // イベントの解除
                var callback = GetLayoutUpdatedCallback(grid);
                grid.LayoutUpdated -= callback;

                // AutoFill処理のリセット
                ClearAutoFill(grid);
            }
        }

        private static LayoutUpdateEventHandler CreateLayoutUpdateHandler(Grid grid)
        {
            var prevCount = 0;
            var prevColumn = grid.ColumnDefinitions.Count;
            var prevRow = grid.RowDefinitions.Count;
            var prevOrientation = GetAutoFillOrientation(grid);

            var layoutUpdateCallback = new LayoutUpdateEventHandler((sender, args) =>
            {
                var count = grid.Children.Count;
                var column = grid.ColumnDefinitions.Count;
                var row = grid.RowDefinitions.Count;
                var orientation = GetAutoFillOrientation(grid);

                if (count != prevCount ||
                    column != prevColumn ||
                    row != prevRow ||
                    orientation != prevOrientation)
                {
                    AutoFill(grid);
                    prevCount = count;
                    prevColumn = column;
                    prevRow = row;
                    prevOrientation = orientation;
                }
            });

            return layoutUpdateCallback;
        }

        public static LayoutUpdateEventHandler GetLayoutUpdatedCallback(AvaloniaObject obj)
        {
            return obj.GetValue(LayoutUpdatedCallbackProperty);
        }
        private static void SetLayoutUpdatedCallback(AvaloniaObject obj, LayoutUpdateEventHandler value)
        {
            obj.SetValue(LayoutUpdatedCallbackProperty, value);
        }

        // Using a AvaloniaProperty as the backing store for LayoutUpdatedCallback.  This enables animation, styling, binding, etc...
        public static readonly AvaloniaProperty<LayoutUpdateEventHandler> LayoutUpdatedCallbackProperty =
            AvaloniaProperty.RegisterAttached<GridEx, Control, LayoutUpdateEventHandler>("LayoutUpdatedCallback", null);

        private static void AutoFill(Grid grid)
        {
            Debug.WriteLine($"Auto{grid.Name}");
            var isEnabled = GetAutoFillChildren(grid);
            var rowCount = grid.RowDefinitions.Count;
            var columnCount = grid.ColumnDefinitions.Count;
            var orientation = GetAutoFillOrientation(grid);


            Debug.WriteLine($"rowCount {rowCount}");
            Debug.WriteLine($"columnCount {columnCount}");

            if (!isEnabled || rowCount == 0 || columnCount == 0) return;

            var area = new bool[rowCount, columnCount];

            var autoLayoutList = new List<Control>();

            // Grid内の位置固定要素のチェック
            foreach (Control child in grid.Children)
            {
                // AreaName ⇒ Areaの優先順位で、グリッド位置の設定を行う
                var region = GetAreaNameRegion(child) ?? GetAreaRegion(child);
                var isFixed = region != null;

                if (isFixed)
                {
                    // 位置指定されているので、AutoFillReservedAreaに記録する
                    var row = region.Row;
                    var column = region.Column;
                    var rowSpan = region.RowSpan;
                    var columnSpan = region.ColumnSpan;

                    for (var i = row; i < row + rowSpan; i++)
                        for (var j = column; j < column + columnSpan; j++)
                        {
                            if (columnCount <= j || rowCount <= i) { continue; }
                            area[i, j] = true;
                        }
                }
                else
                {
                    // Gridの位置未設定の要素は、自動レイアウト対象としてリストに追加
                    autoLayoutList.Add(child);
                }

            }

            var count = 0;
            var numOfCell = rowCount * columnCount;
            var isHorizontal = orientation == Orientation.Horizontal;
            var isOverflow = false;
            // Gridの子要素を、順番にGrid内に並べていく
            foreach (Control child in autoLayoutList)
            {
                // Visibility.Collapsedの項目は除外する
                if (child.IsVisible == false)
                {
                    continue;
                }

                while (true)
                {
                    var x = isHorizontal ? count % columnCount : count / rowCount;
                    var y = isHorizontal ? count / columnCount : count % rowCount;
                    var canArrange = isOverflow ? true : !area[y, x];
                    if (canArrange)
                    {
                        Grid.SetRow(child, y);
                        Grid.SetColumn(child, x);
                        Grid.SetRowSpan(child, 1);
                        Grid.SetColumnSpan(child, 1);
                    }

                    if (count + 1 < numOfCell)
                    {
                        count++;
                    }
                    else
                    {
                        isOverflow = true;
                    }

                    if (canArrange)
                    {
                        break;
                    }
                }

            }
        }

        private static void ClearAutoFill(Grid grid)
        {
            foreach (Control child in grid.Children)
            {
                child.ClearValue(Grid.RowProperty);
                child.ClearValue(Grid.ColumnProperty);
                child.ClearValue(Grid.RowSpanProperty);
                child.ClearValue(Grid.ColumnSpanProperty);

                UpdateItemPosition(child);
            }
        }

        // ↓GridEx内部でだけ使用する、プライベートな添付プロパティ
        public static IList<NamedAreaDefinition> GetAreaDefinitions(AvaloniaObject obj)
        {
            return obj.GetValue(AreaDefinitionsProperty);
        }
        private static void SetAreaDefinitions(AvaloniaObject obj, IList<NamedAreaDefinition> value)
        {
            obj.SetValue(AreaDefinitionsProperty, value);
        }
        // Using a AvaloniaProperty as the backing store for AreaDefinitions.  This enables animation, styling, binding, etc...
        public static readonly AvaloniaProperty<IList<NamedAreaDefinition>> AreaDefinitionsProperty =
                    AvaloniaProperty.RegisterAttached<GridEx, Control, IList<NamedAreaDefinition>>("AreaDefinitions", null);
        public static string GetTemplateArea(AvaloniaObject obj)
        {
            return obj.GetValue(TemplateAreaProperty);
        }
        public static void SetTemplateArea(AvaloniaObject obj, string value)
        {
            obj.SetValue(TemplateAreaProperty, value);
        }

        private static void ReevalTemplateArea(object sender, VisualTreeAttachmentEventArgs e)
        {
            var g = sender as Grid;
            if (g != null)
            {
                InitializeTemplateArea(g, g.GetValue(TemplateAreaProperty));
            }
        }

        // Using a AvaloniaProperty as the backing store for TemplateArea.  This enables animation, styling, binding, etc...
        public static readonly AvaloniaProperty<string> TemplateAreaProperty =
                    AvaloniaProperty.RegisterAttached<GridEx, Control, string>("TemplateArea", null);
        private static void OnTemplateAreaChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var grid = e.Sender as Grid;
            var param = e.NewValue as string;

            if (grid == null)
            {
                return;
            }

            grid.AttachedToVisualTree -= ReevalTemplateArea;
            grid.AttachedToVisualTree += ReevalTemplateArea;

            // グリッドを一度初期化
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            if (param != null)
            {
                InitializeTemplateArea(grid, param);
            }


        }

        private static void InitializeTemplateArea(Grid grid, string param)
        {
            // 行×列数のチェック
            // 空行や、スペースを除去して、行×列のデータ構造に変形
            var columns = param.Split(new[] { '\n', '/' })
                               .Select(o => o.Trim())
                               .Where(o => !string.IsNullOrWhiteSpace(o))
                               .Select(o => o.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            // 行×列数のチェック
            var num = columns.FirstOrDefault().Count();
            var isValidRowColumn = columns.All(o => o.Count() == num);
            if (!isValidRowColumn)
            {
                // Invalid Row Columns...
                throw new ArgumentException("Invalid Row/Column definition.");
            }

            // グリッド数を調整(不足分の行/列を足す)
            var rowShortage = columns.Count() - grid.RowDefinitions.Count;
            for (var i = 0; i < rowShortage; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            var columnShortage = num - grid.ColumnDefinitions.Count;
            for (var i = 0; i < columnShortage; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Area定義をパース
            var areaList = ParseAreaDefinition(columns);
            SetAreaDefinitions(grid, areaList);

            // 全体レイアウトの定義が変わったので、
            // Gridの子要素のすべてのRegion設定を反映しなおす
            foreach (Control child in grid.Children)
            {
                UpdateItemPosition(child);
            }
        }


        private static IList<NamedAreaDefinition> ParseAreaDefinition(IEnumerable<string[]> columns)
        {
            var result = new List<NamedAreaDefinition>();

            // Regionが正しく連結されているかチェック
            var flatten = columns.SelectMany(
                    (item, index) => item.Select((o, xIndex) => new { row = index, column = xIndex, name = o })
                );

            var groups = flatten.GroupBy(o => o.name);
            foreach (var group in groups)
            {
                var left = group.Min(o => o.column);
                var top = group.Min(o => o.row);
                var right = group.Max(o => o.column);
                var bottom = group.Max(o => o.row);

                var isValid = true;
                for (var y = top; y <= bottom; y++)
                    for (var x = left; x <= right; x++)
                    {
                        isValid = isValid && group.Any(o => o.column == x && o.row == y);
                    }

                if (!isValid)
                {
                    throw new ArgumentException($"\"{group.Key}\" is invalid area definition.");
                }

                result.Add(new NamedAreaDefinition(group.Key, top, left, bottom - top + 1, right - left + 1));
            }

            return result;
        }

        private static GridLengthDefinition StringToGridLengthDefinition(string source)
        {
            var r = new System.Text.RegularExpressions.Regex(@"(^[^\(\)]+)(?:\((.*)-(.*)\))?");
            var m = r.Match(source);

            var length = m.Groups[1].Value;
            var min = m.Groups[2].Value;
            var max = m.Groups[3].Value;

            double temp;
            var result = new GridLengthDefinition()
            {
                GridLength = StringToGridLength(length),
                Min = double.TryParse(min, out temp) ? temp : (double?)null,
                Max = double.TryParse(max, out temp) ? temp : (double?)null
            };

            return result;
        }


        private static GridLength StringToGridLength(string source)
        {
            var glc = TypeDescriptor.GetConverter(typeof(GridLength));
            return (GridLength)glc.ConvertFromString(source);
        }




        //=====================================================================
        // Grid内の子要素に適用するための添付プロパティ類
        //=====================================================================
        public static string GetAreaName(AvaloniaObject obj)
        {
            return obj.GetValue(AreaNameProperty);
        }
        public static void SetAreaName(AvaloniaObject obj, string value)
        {
            obj.SetValue(AreaNameProperty, value);
        }

        // Using a AvaloniaProperty as the backing store for AreaName.  This enables animation, styling, binding, etc...
        public static readonly AvaloniaProperty<string> AreaNameProperty =
                    AvaloniaProperty.RegisterAttached<GridEx, Control, string>("AreaName", null);

        private static void OnAreaNameChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var ctrl = e.Sender as Control;

            if (ctrl == null)
            {
                return;
            }


            // 子要素全体のAutoFillを計算しなおす
            var grid = ctrl.Parent as Grid;
            if (grid == null)
            {
                return;
            }

            UpdateItemPosition(ctrl);

            var isAutoFill = GetAutoFillChildren(grid);
            if (isAutoFill)
            {
                AutoFill(grid);
            }
        }

        public static string GetArea(AvaloniaObject obj)
        {
            return (string)obj.GetValue(AreaProperty);
        }
        public static void SetArea(AvaloniaObject obj, string value)
        {
            obj.SetValue(AreaProperty, value);
        }
        // Using a AvaloniaProperty as the backing store for Area.  This enables animation, styling, binding, etc...
        public static readonly AvaloniaProperty<string> AreaProperty =
                     AvaloniaProperty.RegisterAttached<GridEx, Control, string>("Area", null);

        private static void OnAreaChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var ctrl = e.Sender as Control;

            if (ctrl == null)
            {
                return;
            }


            // 子要素全体のAutoFillを計算しなおす
            var grid = ctrl.Parent as Grid;
            if (grid == null)
            {
                return;
            }

            UpdateItemPosition(ctrl);

            var isAutoFill = GetAutoFillChildren(grid);
            if (isAutoFill)
            {
                AutoFill(grid);
            }
        }


        private static void UpdateItemPosition(Control element)
        {
            // AreaName ⇒ Areaの優先順位で、グリッド位置の設定を行う
            var area = GetAreaNameRegion(element) ?? GetAreaRegion(element);
            if (area != null)
            {
                Grid.SetRow(element, area.Row);
                Grid.SetColumn(element, area.Column);
                Grid.SetRowSpan(element, area.RowSpan);
                Grid.SetColumnSpan(element, area.ColumnSpan);
            }
        }


        private static AreaDefinition GetAreaNameRegion(Control element)
        {
            var name = GetAreaName(element);
            var grid = element.Parent as Grid;
            if (grid == null || name == null) { return null; }
            var areaList = GetAreaDefinitions(grid);
            if (areaList == null) { return null; }

            var area = areaList.FirstOrDefault(o => o.Name == name);
            if (area == null) { return null; }

            return new AreaDefinition(area.Row, area.Column, area.RowSpan, area.ColumnSpan);
        }

        private static AreaDefinition GetAreaRegion(Control element)
        {
            var param = GetArea(element);
            if (param == null) { return null; }

            var list = param.Split(',')
                .Select(o => o.Trim())
                .Select(o => int.Parse(o))
                .ToList();

            // Row, Column, RowSpan, ColumnSpan
            if (list.Count() != 4) { return null; }

            return new AreaDefinition(list[0], list[1], list[2], list[3]);
        }
    }
}
