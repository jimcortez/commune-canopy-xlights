
/*{
    "DESCRIPTION": "",
    "CREDIT": "",
    "ISFVSN": "2",
    "CATEGORIES": [
        "XXX"
    ],
    "INPUTS": [
    ],
   
    
}*/

#define iTime (TIME/20.0)   // adjust divisor to speed up or slow down

float sdCircle( in vec2 p, in float r ) 
{
    return length(p)-r;
}

//https://iquilezles.org/articles/palettes/
vec3 palette( float t ) {
//    vec3 a = vec3(0.5, 0.5, 0.5);
//    vec3 b = vec3(0.5, 0.5, 0.5);
//    vec3 c = vec3(1.0, 1.0, 1.0);
//    vec3 d = vec3(0.263,0.416,0.557);

    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 0.6);
    vec3 d = vec3(0.80, 0.90, 0.30);
    //vec3 d = vec3(0.3, 0.2, 0.2);
    //vec3 d = vec3(0.3, 0.2, 0.2);

    return a + b*cos( 6.28318*(c*t+d) );
}

void main() {
    vec2 p = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
    vec3 col;
    vec2 vec;
    float d;
    for(float i=0.0;i<60.0;i++)
    {
        vec=vec2(cos(i*iTime)*(i*0.04),sin(i*iTime)*(i*0.04));

        d=sdCircle(p+vec,i*0.07);

        col=(d>0.0) ? palette(0.6* mod(i,10.0)): col;
        col = mix(col, vec3(0.0), smoothstep(0.01,0.0,abs(d)) );

    }
    gl_FragColor = vec4(col,1.0);
}
