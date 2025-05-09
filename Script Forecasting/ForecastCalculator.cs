using UnityEngine;
public static class ForecastCalculator
{
    public static (float[], float[], float, float, float) CalculateMovingAverage(float[] demandValues, int period)
    {
        float[] forecastResults = new float[demandValues.Length];
        float[] errors = new float[demandValues.Length];
        float mad = 0f, mse = 0f, mape = 0f;

        for (int i = 0; i < demandValues.Length; i++)
        {
            if (i < period)
            {
                forecastResults[i] = demandValues[i]; // Gunakan demand langsung jika data kurang
            }
            else
            {
                float sum = 0f;
                for (int j = 0; j < period; j++)
                {
                    sum += demandValues[i - j];
                }
                forecastResults[i] = sum / period;
            }

            errors[i] = demandValues[i] - forecastResults[i];
            mad += Mathf.Abs(errors[i]);
            mse += Mathf.Pow(errors[i], 2);
            mape += Mathf.Abs(errors[i] / demandValues[i]) * 100;
        }

        return (forecastResults, errors, mad / demandValues.Length, mse / demandValues.Length, mape / demandValues.Length);
    }

    public static (float[], float[], float, float, float) CalculateExponentialSmoothing(float[] demandValues, float alpha)
    {
        float[] forecastResults = new float[demandValues.Length];
        float[] errors = new float[demandValues.Length];
        float mad = 0f, mse = 0f, mape = 0f;

        forecastResults[0] = demandValues[0]; // Forecast pertama sama dengan demand pertama

        for (int i = 1; i < demandValues.Length; i++)
        {
            forecastResults[i] = alpha * demandValues[i - 1] + (1 - alpha) * forecastResults[i - 1];
            errors[i] = demandValues[i] - forecastResults[i];

            mad += Mathf.Abs(errors[i]);
            mse += Mathf.Pow(errors[i], 2);
            mape += Mathf.Abs(errors[i] / demandValues[i]) * 100;
        }

        return (forecastResults, errors, mad / demandValues.Length, mse / demandValues.Length, mape / demandValues.Length);
    }
}