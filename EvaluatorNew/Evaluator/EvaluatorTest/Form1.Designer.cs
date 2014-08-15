namespace EvaluatorTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StaticLabelExpression = new System.Windows.Forms.Label();
            this.TextBoxExpression = new System.Windows.Forms.TextBox();
            this.LabelResult = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StaticLabelExpression
            // 
            this.StaticLabelExpression.AutoSize = true;
            this.StaticLabelExpression.Location = new System.Drawing.Point(13, 13);
            this.StaticLabelExpression.Name = "StaticLabelExpression";
            this.StaticLabelExpression.Size = new System.Drawing.Size(65, 13);
            this.StaticLabelExpression.TabIndex = 0;
            this.StaticLabelExpression.Text = "Expression:";
            // 
            // TextBoxExpression
            // 
            this.TextBoxExpression.Location = new System.Drawing.Point(84, 10);
            this.TextBoxExpression.Name = "TextBoxExpression";
            this.TextBoxExpression.Size = new System.Drawing.Size(238, 22);
            this.TextBoxExpression.TabIndex = 1;
            this.TextBoxExpression.TextChanged += new System.EventHandler(this.TextBoxExpression_TextChanged);
            // 
            // LabelResult
            // 
            this.LabelResult.AutoSize = true;
            this.LabelResult.Location = new System.Drawing.Point(13, 44);
            this.LabelResult.Name = "LabelResult";
            this.LabelResult.Size = new System.Drawing.Size(0, 13);
            this.LabelResult.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 66);
            this.Controls.Add(this.LabelResult);
            this.Controls.Add(this.TextBoxExpression);
            this.Controls.Add(this.StaticLabelExpression);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Evaluator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StaticLabelExpression;
        private System.Windows.Forms.TextBox TextBoxExpression;
        private System.Windows.Forms.Label LabelResult;
    }
}

