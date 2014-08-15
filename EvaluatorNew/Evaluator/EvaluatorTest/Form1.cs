using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Evaluator;

namespace EvaluatorTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void TextBoxExpression_TextChanged(object sender, EventArgs e)
        {
            string text = this.TextBoxExpression.Text;
            BigDecimal value;
            BigDecimal.TryParse(text, out value);
            string result = (value != 0) ? BigDecimal.Ln(value).ToString() : "0";
            this.LabelResult.Text = result;
        }
    }
}
