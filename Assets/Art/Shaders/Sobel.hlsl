

void Sobel_float(float3 color, float2 uv, float2 viewsize, out float3 Out) 
{
	float gradientX = 0;
	float gradientY = 0;

	float sobelX[9] = { -1,-2,-1,0,0,0,1,2,1 };
	float sobelY[9] = { -1,0,1,-2,0,2,-1,0,1 };

	int index = 0;

	float texelSize = 1 / viewsize;

	for (int x = -1; x <= 1; x++)
	{
		for (int y = -1; y <= 1; y++)
		{
			if (index == 4) 
			{
				index++;
				continue;
			}
			float2 offset = float2(x, y) * texelSize;
			float3 pxCol = SHADERGRAPH_SAMPLE_SCENE_COLOR(uv + offset);
			float pxLum = dot(pxCol, float3(0.2126, 0.7152, 0.0722));

			gradientX += pxLum * sobelX[index];
			gradientY += pxLum * sobelY[index];

			index++;
		}
	}

	float angle = 0;
	if (abs(gradientY / gradientX) > 0.001) 
	{
		angle = atan(gradientY / gradientX);
	}
		

	Out = angle;
}