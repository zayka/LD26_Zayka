//float4x4 World;
//float4x4 View;
//float4x4 Projection;


sampler  ColorSampler  : register(s0);
sampler  ColorSampler2  : register(s1);





float BloomThreshold;

float BloomIntensity;
float BaseIntensity;

float BloomSaturation;
float BaseSaturation;

float w0;
float w1;
float w2;
float w3;
float w4;

float o1;
float o2;
float o3;
float o4;

float deltaX;
float deltaY;


struct PS_Input
{
	float4 color    : COLOR0;
	float2 TexCoords : TEXCOORD0;
};

float4 FilterPS(PS_Input input) : COLOR0
{
 // Look up the original image color.
    float4 c = tex2D(ColorSampler, input.TexCoords);

    // Adjust it to keep only values brighter than the specified threshold.

    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}

float4 BlurPS(PS_Input input) : COLOR0
{
    float step = 1.0;
    //float2 deltaX = float2( step / 1024, 0);
    //float2 deltaY = float2( 0, step / 768);    
	float2 delta = float2(deltaX,deltaY);
    float4 color = tex2D(ColorSampler, input.TexCoords) * w0;

		color+=tex2D(ColorSampler, input.TexCoords+o1*delta)*w1;
		color+=tex2D(ColorSampler, input.TexCoords-o1*delta)*w1;
		
		color+=tex2D(ColorSampler, input.TexCoords+o2*delta)*w2;
		color+=tex2D(ColorSampler, input.TexCoords-o2*delta)*w2;

		color+=tex2D(ColorSampler, input.TexCoords+o3*delta)*w3;
		color+=tex2D(ColorSampler, input.TexCoords-o3*delta)*w3;

		color+=tex2D(ColorSampler, input.TexCoords+o4*delta)*w4;
		color+=tex2D(ColorSampler, input.TexCoords-o4*delta)*w4;
			
    return color;
}

// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}


float4 CombinePS(PS_Input input) : COLOR0
{
	 // Look up the bloom and original base image colors. 
	float4 newBase = tex2D(ColorSampler2, input.TexCoords);  
	float4 newBloom = tex2D(ColorSampler, input.TexCoords);  	
    // Adjust color saturation and intensity.
	
    newBloom = AdjustSaturation(newBloom, BloomSaturation) * BloomIntensity;
    newBase = AdjustSaturation(newBase, BaseSaturation) * BaseIntensity;
    
    // Darken down the base image in areas where there is a lot of bloom,
    // to prevent things looking excessively burned-out.
    //newBase *= (1.0 - saturate(newBloom));
    
    // Combine the two images. 
	return newBloom+newBase;
	
}


technique Filter
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 FilterPS();
    }
}


technique Blur
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 BlurPS();
    }
}

technique Combine
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 CombinePS();
    }
}
