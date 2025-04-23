void Kuwahara_float(UnityTexture2D blit,UnitySamplerState ss, float2 uv, float2 viewsize,float2 radius, out float3 Out) 
{
	//Kuwahara Variables
	float3 mean[4] = {
		{0,0,0},
		{0,0,0},
		{0,0,0},
		{0,0,0}
	};
	float3 sigma[4] = {
		{0,0,0},
		{0,0,0},
		{0,0,0},
		{0,0,0}
	};
	float2 offsets[4] = {
		{-radius.x,-radius.y},
		{-radius.x,0},
		{0,-radius.y},
		{0,0}
	};
	float2 pos;
	float3 col;

	//Sobel Variables
	float gradientX = 0;
	float gradientY = 0;

	float sobelX[9] = { -1,-2,-1,0,0,0,1,2,1 };
	float sobelY[9] = { -1,0,1,-2,0,2,-1,0,1 };

	int index = 0;

	float2 texelSize = 1 / viewsize;



	//Sobel Calculation
	[loop]
	for (int x = -1; x <= 1; x++)
	{
		[loop]
		for (int y = -1; y <= 1; y++)
		{
			[branch]
			if (index == 4)
			{
				index++;
				continue;
			}
			float2 offset = float2(x, y) * texelSize;
			float3 pxCol = SAMPLE_TEXTURE2D_LOD(blit,ss, uv + offset, 0);
			float pxLum = dot(pxCol, float3(0.2126, 0.7152, 0.0722));

			gradientX += pxLum * sobelX[index];
			gradientY += pxLum * sobelY[index];

			index++;
		}
	}

	float angle = 0;
	[branch]
	if (abs(gradientY / gradientX) > 0.001)
	{
		angle = atan(gradientY / gradientX);
	}

	float s = sin(angle);
	float c = cos(angle);

	// Kuwahara Calculation
	[loop]
	for (int i = 0; i < 4; i++)
	{
		[loop]
		for (int j = 0; j < radius.x; j++)
		{
			[loop]
			for (int k = 0; k < radius.y; k++)
			{
				pos = float2(j, k) + offsets[i];
				float2 offs = pos * texelSize;
				offs = float2(offs.x * c - offs.y * s, offs.x * s + offs.y * c);
				float2 uvpos = uv + offs;
				col = SAMPLE_TEXTURE2D_LOD(blit, ss, uvpos.xy, 0);

				col = saturate(col);

				// Compute distance from center
				float dist = length(pos); // or length(offs / texelSize) if you want texel-relative distance

				// Weight: you can try different falloff formulas
				float weight = exp(-dist * dist * 0.02); // Gaussian-like falloff
				// float weight = 1.0 / (1.0 + dist); // Inverse falloff
				// float weight = saturate(1.0 - dist / maxDistance); // Linear falloff

				mean[i] += col * weight;
				sigma[i] += col * col * weight;
			}
		}
	}

	float n = (radius.x) * (radius.y);
	float sigma_f;
	float min = 1;
	[loop]
	for (int l = 0; l < 4; l++) 
	{
		mean[l] /= n;
		sigma[l] = abs(sigma[l] / n - mean[l] * mean[l]) ;
		sigma_f = sigma[l].r + sigma[l].g + sigma[l].b;
		[branch]
		if (sigma_f < min)
		{
			min = sigma_f;
			col = mean[l];
			//col = sigma_f * 100;
		}
	}

	//TEXTURE2D_X(_BlitTexture);

	//uint2 pixelCoords = uint2(uv * viewsize.xy);
	Out = col;

	//Out = color.Sample(ss,uv);
}