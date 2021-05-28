﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using OfficeOpenXml.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPPlusTest.Sorting
{
    [TestClass]
    public class SortRangeTests
    {
        [TestMethod]
        public void ShouldSetSortState()
        {
            using (var package = new ExcelPackage())
            {
                var rnd = new Random();
                var sheet = package.Workbook.Worksheets.Add("Test");
                Assert.IsNull(sheet.SortState, "Worksheet.SortState was not null");
                for (var x = 1; x < 10; x++)
                {
                    sheet.Cells[x, 1].Value = rnd.Next(1, 20);
                }
                sheet.Cells["A1:A10"].Sort(0, true);
                Assert.IsNotNull(sheet.SortState, "Worksheet.SortState was null");
                Assert.AreEqual(1, sheet.SortState.SortConditions.Count());
            }
        }

        [TestMethod]
        public void ShouldSortMultipleColumns()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");
                sheet.Cells[1, 1].Value = 1;
                sheet.Cells[2, 1].Value = 6;
                sheet.Cells[3, 1].Value = 2;
                sheet.Cells[4, 1].Value = 1;
                sheet.Cells[1, 2].Value = 1;
                sheet.Cells[2, 2].Value = 2;
                sheet.Cells[3, 2].Value = 3;
                sheet.Cells[4, 2].Value = 4;
                sheet.Cells[1, 3].Value = "A";
                sheet.Cells[2, 3].Value = "D";
                sheet.Cells[3, 3].Value = "C";
                sheet.Cells[4, 3].Value = "B";
                sheet.Cells["A1:C4"].Sort(new int[2] { 0, 2 }, new bool[2] { false, true });
                Assert.IsNotNull(sheet.SortState);
                Assert.AreEqual(2, sheet.SortState.SortConditions.Count());
            }
        }

        [TestMethod]
        public void ShouldSortSingleColumn()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");
                sheet.Cells[1, 1].Value = 4;
                sheet.Cells[2, 1].Value = 3;
                sheet.Cells[3, 1].Value = 2;

                // sort ascending
                sheet.Cells["A1:A3"].Sort();
                Assert.AreEqual(2, sheet.Cells["A1"].Value);
                Assert.AreEqual(3, sheet.Cells["A2"].Value);
                Assert.AreEqual(4, sheet.Cells["A3"].Value);

                sheet.Cells[1, 1].Value = 3;
                sheet.Cells[2, 1].Value = 4;
                sheet.Cells[3, 1].Value = 2;

                // sort descending
                sheet.Cells["A1:A3"].Sort(0, true);
                Assert.AreEqual(4, sheet.Cells["A1"].Value);
                Assert.AreEqual(3, sheet.Cells["A2"].Value);
                Assert.AreEqual(2, sheet.Cells["A3"].Value);
            }
        }

        [TestMethod]
        public void ShouldConfigureSortOptionsForSingleColumn()
        {
            var options = new RangeSortOptions();
            options.SortBy.Column(0);
            Assert.AreEqual(1, options.ColumnIndexes.Count);
            Assert.AreEqual(1, options.Descending.Count);
            Assert.AreEqual(0, options.ColumnIndexes.First());
            Assert.IsFalse(options.Descending.First());
        }

        [TestMethod]
        public void ShouldConfigureSortOptionsForMultiColumns()
        {
            var options = new RangeSortOptions();
            options
                .SortBy.Column(0)
                .ThenSortBy.Column(3, eSortDirection.Descending)
                .ThenSortBy.Column(2);
            Assert.AreEqual(3, options.ColumnIndexes.Count);
            Assert.AreEqual(3, options.Descending.Count);
            Assert.AreEqual(0, options.ColumnIndexes.First());
            Assert.IsFalse(options.Descending.First());
            Assert.AreEqual(3, options.ColumnIndexes[1]);
            Assert.IsTrue(options.Descending[1]);
        }

        [TestMethod]
        public void ShouldSortMultipleColumnsWithOptions()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");
                sheet.Cells[1, 1].Value = 1;
                sheet.Cells[2, 1].Value = 6;
                sheet.Cells[3, 1].Value = 2;
                sheet.Cells[4, 1].Value = 1;
                sheet.Cells[1, 2].Value = 1;
                sheet.Cells[2, 2].Value = 2;
                sheet.Cells[3, 2].Value = 3;
                sheet.Cells[4, 2].Value = 4;
                sheet.Cells[1, 3].Value = "A";
                sheet.Cells[2, 3].Value = "D";
                sheet.Cells[3, 3].Value = "C";
                sheet.Cells[4, 3].Value = "B";

                sheet.Cells["A1:C4"].Sort(options => options
                                                        .SortBy.Column(0)
                                                        .ThenSortBy.Column(2, eSortDirection.Descending));
                Assert.IsNotNull(sheet.SortState);
                Assert.AreEqual(2, sheet.SortState.SortConditions.Count());
                Assert.IsTrue(sheet.SortState.SortConditions.Last().Descending);
                Assert.AreEqual(6, sheet.Cells[4, 1].Value);
                Assert.AreEqual("B", sheet.Cells[1, 3].Value);
            }
        }

        [TestMethod]
        public void ShouldSortByCustomList()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");
                sheet.Cells[1, 1].Value = "Blue";
                sheet.Cells[2, 1].Value = "Red";
                sheet.Cells[3, 1].Value = "Yellow";
                sheet.Cells[4, 1].Value = "Blue";

                sheet.Cells[1, 2].Value = 2;
                sheet.Cells[2, 2].Value = 1;
                sheet.Cells[3, 2].Value = 1;
                sheet.Cells[4, 2].Value = 1;

                sheet.Cells["A1:B4"].Sort(x => x.SortBy.Column(0).UsingCustomList("Red", "Yellow", "Blue").ThenSortBy.Column(1));

                Assert.AreEqual("Red", sheet.Cells[1, 1].Value);
                Assert.AreEqual("Yellow", sheet.Cells[2, 1].Value);
                Assert.AreEqual("Blue", sheet.Cells[3, 1].Value);
                Assert.AreEqual(1, sheet.Cells[3, 2].Value);
                Assert.AreEqual("Blue", sheet.Cells[4, 1].Value);
                Assert.AreEqual(2, sheet.Cells[4, 2].Value);
            }
        }

        [TestMethod]
        public void ShouldSortLeftToRight()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");
                sheet.Cells[1, 1].Value = 4;
                sheet.Cells[1, 2].Value = 1;
                sheet.Cells[1, 3].Value = 5;
                sheet.Cells[1, 4].Value = 2;
                sheet.Cells[1, 5].Value = 3;
                sheet.Cells[2, 1].Value = 4;
                sheet.Cells[2, 2].Value = 1;
                sheet.Cells[2, 3].Value = 5;
                sheet.Cells[2, 4].Value = 2;
                sheet.Cells[2, 5].Value = 3;
                sheet.Cells["A1:E2"].Sort(x => x.SortLeftToRightBy.Row(0));

                Assert.AreEqual(1, sheet.Cells[1, 1].Value);
                Assert.AreEqual(2, sheet.Cells[1, 2].Value);
                Assert.AreEqual(5, sheet.Cells[1, 5].Value);
                Assert.AreEqual(1, sheet.Cells[2, 1].Value);
                Assert.AreEqual(2, sheet.Cells[2, 2].Value);
                Assert.AreEqual(5, sheet.Cells[2, 5].Value);
            }
        }

        [TestMethod]
        public void ShouldSetStortStateLeftToRight()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");
                sheet.Cells[1, 1].Value = 4;
                sheet.Cells[1, 2].Value = 1;
                sheet.Cells[1, 3].Value = 5;
                sheet.Cells[1, 4].Value = 2;
                sheet.Cells[1, 5].Value = 3;
                sheet.Cells[2, 1].Value = 4;
                sheet.Cells[2, 2].Value = 1;
                sheet.Cells[2, 3].Value = 5;
                sheet.Cells[2, 4].Value = 2;
                sheet.Cells[2, 5].Value = 3;
                sheet.Cells["A1:E2"].Sort(x => x.SortLeftToRightBy.Row(0));

                Assert.AreEqual("A1:E2", sheet.SortState.Ref);
                Assert.IsTrue(sheet.SortState.ColumnSort);
                Assert.AreEqual("A1:E1", sheet.SortState.SortConditions.ElementAt(0).Ref);
            }
        }

        [TestMethod]
        public void ShouldSortLeftToRightWithTwoLayers()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");
                sheet.Cells[1, 1].Value = 4;
                sheet.Cells[1, 2].Value = 4;
                sheet.Cells[1, 3].Value = 4;
                sheet.Cells[1, 4].Value = 4;
                sheet.Cells[1, 5].Value = 3;
                sheet.Cells[2, 1].Value = 4;
                sheet.Cells[2, 2].Value = 1;
                sheet.Cells[2, 3].Value = 3;
                sheet.Cells[2, 4].Value = 2;
                sheet.Cells[2, 5].Value = 5;
                sheet.Cells["A1:E2"].Sort(x => x.SortLeftToRightBy.Row(0).ThenSortBy.Row(1));

                Assert.AreEqual(3, sheet.Cells[1, 1].Value);
                Assert.AreEqual(4, sheet.Cells[1, 2].Value);
                Assert.AreEqual(4, sheet.Cells[1, 5].Value);
                Assert.AreEqual(5, sheet.Cells[2, 1].Value);
                Assert.AreEqual(1, sheet.Cells[2, 2].Value);
                Assert.AreEqual(4, sheet.Cells[2, 5].Value);
            }
        }

        [TestMethod]
        public void ShouldShiftRowsInSharedFormula()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");
                sheet.Cells[1, 1].Value = 3;
                sheet.Cells[2, 1].Value = 2;
                sheet.Cells[3, 1].Value = 1;

                sheet.Cells[1, 2, 3, 2].Formula = "SUM(A1)";
                sheet.Cells["A1:B3"].Sort(x => x.SortBy.Column(0));

                Assert.AreEqual(1, sheet.Cells["A1"].Value);
                Assert.AreEqual("SUM(A1)", sheet.Cells[1, 2].Formula);
                Assert.AreEqual("SUM(A2)", sheet.Cells[2, 2].Formula);
                Assert.AreEqual("SUM(A3)", sheet.Cells[3, 2].Formula);
            }
        }

        [TestMethod]
        public void ShouldShiftRowsInFormula()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");

                sheet.Cells[1, 1].Value = 3;
                sheet.Cells[2, 1].Value = 2;
                sheet.Cells[3, 1].Value = 1;
                
                sheet.Cells[1, 2].Formula = "SUM(A1)";
                sheet.Cells[2, 2].Formula = "SUM(A2)";
                sheet.Cells[3, 2].Formula = "SUM(A3)";

                sheet.Cells["A1:B3"].Sort(x => x.SortBy.Column(0));

                Assert.AreEqual("SUM(A1)", sheet.Cells[1, 2].Formula);
            }
        }

        [TestMethod]
        public void ShouldShiftRowsInFormulaWithRangeAddress()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");

                sheet.Cells[1, 1].Value = 3;
                sheet.Cells[2, 1].Value = 2;
                sheet.Cells[3, 1].Value = 1;

                sheet.Cells[1, 2].Value = 3;
                sheet.Cells[2, 2].Value = 2;
                sheet.Cells[3, 2].Value = 1;

                sheet.Cells[1, 3].Formula = "SUM(A1:B1)";
                sheet.Cells[2, 3].Formula = "SUM(A2:B2)";
                sheet.Cells[3, 3].Formula = "SUM(A3:B3)";

                sheet.Cells["A1:B3"].Sort(x => x.SortBy.Column(0));

                Assert.AreEqual("SUM(A1:B1)", sheet.Cells[1, 3].Formula);
            }
        }

        [TestMethod]
        public void ShouldShiftColumnsInFormulaWithRangeAddress()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");

                sheet.Cells[1, 1].Value = 3;
                sheet.Cells[1, 2].Value = 2;
                sheet.Cells[1, 3].Value = 1;

                sheet.Cells[2, 1].Value = 3;
                sheet.Cells[2, 2].Value = 2;
                sheet.Cells[2, 3].Value = 1;

                sheet.Cells[3, 1].Formula = "SUM(A1:A2)";
                sheet.Cells[3, 2].Formula = "SUM(B1:B2)";
                sheet.Cells[3, 3].Formula = "SUM(C1:C2)";

                sheet.Cells["A1:C3"].Sort(x => x.SortLeftToRightBy.Row(0));

                Assert.AreEqual("SUM(A1:A2)", sheet.Cells[3, 1].Formula);
            }
        }

        [TestMethod]
        public void ShouldSortLeftToRightUsingCustomList()
        {
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Test");
                sheet.Cells[1, 1].Value = "S";
                sheet.Cells[1, 2].Value = "M";
                sheet.Cells[1, 3].Value = "M";
                sheet.Cells[1, 4].Value = "L";
                sheet.Cells[1, 5].Value = "L";
                sheet.Cells[2, 1].Value = 4;
                sheet.Cells[2, 2].Value = 1;
                sheet.Cells[2, 3].Value = 3;
                sheet.Cells[2, 4].Value = 2;
                sheet.Cells[2, 5].Value = 5;
                sheet.Cells["A1:E2"].Sort(x => x.SortLeftToRightBy.Row(0).UsingCustomList("S", "M", "L").ThenSortBy.Row(1));

                Assert.AreEqual("S", sheet.Cells[1, 1].Value);
                Assert.AreEqual("M", sheet.Cells[1, 2].Value);
                Assert.AreEqual("L", sheet.Cells[1, 5].Value);
                Assert.AreEqual(4, sheet.Cells[2, 1].Value);
                Assert.AreEqual(1, sheet.Cells[2, 2].Value);
                Assert.AreEqual(5, sheet.Cells[2, 5].Value);
            }
        }
    }
}