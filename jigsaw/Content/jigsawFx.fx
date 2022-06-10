#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

matrix WorldViewProjection;
float2 size;
Texture2D tex;
sampler2D textSamp = sampler_state
{
	Texture = <tex>;
};


struct VertexShaderInput
{
	float4 pos : POSITION0;
	float4 col : COLOR0;
	float2 tex : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 pos : SV_POSITION;
	float4 col : COLOR0;
	float2 tex : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.pos = mul(input.pos, WorldViewProjection);
	output.col = input.col;
	output.tex = input.tex;
	return output;
}

float sdSquircle(float2 pos)
{
	float x = pos.x;
	float y = pos.y;
	x *= x;
	y *= y;
	return pow(x * x + y * y, .25);
}

float mask(float2 pos, int id, int side)
{
	float dist = .3;
	float d = length(pos.x) - dist;
	d = max(d, (length(pos.y) - dist));
	
	if ((side & (1 << 0)) == 0) {
		if ((id & (1 << 0)) != 0)
			d = min(d, (length(pos + float2(dist + .04, .0)) - .1));
		else if ((id & (1 << 0)) == 0)
			d = max(d, -(length(pos + float2(dist - .04, .0)) - .1));
	}
	if ((side & (1 << 1)) == 0) {
		if ((id & (1 << 1)) != 0)
			d = min(d, (length(pos + float2(.0, dist + .04)) - .1));
		else if ((id & (1 << 1)) == 0)
			d = max(d, -(length(pos + float2(.0, dist - .04)) - .1));
	}
	if ((side & (1 << 2)) == 0) {
		if ((id & (1 << 2)) != 0)
			d = min(d, (length(pos + float2(-(dist + .04), .0)) - .1));
		else if ((id & (1 << 2)) == 0)
			d = max(d, -(length(pos + float2(-(dist - .04), .0)) - .1));
	}
	if ((side & (1 << 3)) == 0) {
		if ((id & (1 << 3)) != 0)
			d = min(d, (length(pos + float2(.0, -(dist + .04))) - .1));
		else if ((id & (1 << 3)) == 0)
			d = max(d, -(length(pos + float2(.0, -(dist - .04))) - .1));
	}
	return d;
}

float normalize(float value, float minv, float maxv, float newMin, float newMax)
{
	return (value - minv) * ((newMax - newMin) / (maxv - minv)) + newMin;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 uv = input.tex;// -float2(.2, .2) / float2(7., 5.);
	float2 pos = frac(uv * float2(7., 5.)) - .5; // frac((input.tex / size)* float2(7., 5.));
	
	int id = int(input.col.r * 255);
	int side = int(input.col.g * 255);
	
	float d = mask(pos, id, side);

	float s = smoothstep(.01, .0, d);
	//uv = (input.tex - (float2(.2, .2) / float2(7., 5.)));// / (1 / .6);
	//uv /= .6;

	uv = input.tex * float2(7., 5.);
	float2 iuv = floor(uv);
	float2 fuv = frac(uv);

	fuv.x = normalize(fuv.x, .0, 1., -.33333, 1.33333);
	fuv.y = normalize(fuv.y, .0, 1., -.33333, 1.33333);
	uv = iuv + fuv;
	uv /= float2(7., 5.);

	/*
	fuv -= .5;
	fuv *= (.3 / .5);

	fuv.x = normalize(fuv.x, -.3, .3, -.5, 5);
	fuv.y = normalize(fuv.y, -.3, .3, -.5, 5);

	fuv /= (.3 / .5);
	fuv += .5;

	*/

	float4 col = tex2D(textSamp, uv);// * input.col;
	//col.rb = pos;
	col.a *= s;
	//col.a += .1;
	//if (uv.x < 0 || uv.y < 0) col = 1.;
	//if (uv.x > 1 || uv.y > 1) col = 1.;
	return col;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};