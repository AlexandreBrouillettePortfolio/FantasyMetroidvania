#ifndef UNION_SDF_INCLUDED
#define UNION_SDF_INCLUDED

// Custom function for combining two SDF "shapes" (Top, Bottom) by union.
// Each float4 is assumed to be: (distance, r, g, b).
// Output is (distanceUnion, rUnion, gUnion, bUnion).
float4 UnionSDF_float(float4 Top, float4 Bottom)
{
    // 1) Find the union distance
    float distUnion = min(Top.x, Bottom.x);
    
    // 2) Decide which shape is closer using step()
    //    step(a, b) returns 1 if a >= b, 0 otherwise
    //    We compare Top.x, Bottom.x to see if Top is closer (or equal)
    float useTop = step(Top.x, Bottom.x);
    
    // 3) Lerp color/metadata (yzw) from the shape with the smaller distance
    float3 colorUnion = lerp(Bottom.yzw, Top.yzw, useTop);
    
    // 4) Return the combined shape as float4: (unionDistance, color)
    return float4(distUnion, colorUnion);
}
#endif