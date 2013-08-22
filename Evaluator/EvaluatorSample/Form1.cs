using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Evaluator;

namespace EvaluatorSample
{
    public partial class Form1 : Form
    {
        private EvaluatorCore core;

        public Form1()
        {
            InitializeComponent();
            this.core = new EvaluatorCore();
        }

        private void ExpressionBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string input = this.ExpressionBox.Text;
                if (string.IsNullOrEmpty(input))
                {
                    this.ResultLabel.Text = "";
                    return;
                }

                var expression = new Evaluator.IntegralCore.IntegralExpression();
                expression.Parse(input);
                this.ResultLabel.Text = expression.Evaluate();
            }
            catch (Exception ex)
            {
                this.ResultLabel.Text = ex.Message;
            }
        }

        private void ResultLabel_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.ResultLabel.Text);
        }
    }
}
