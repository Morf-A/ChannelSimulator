function f = createFrequenciesByMEDS(fmax, sinusoidsNumber)
    f = zeros(2,sinusoidsNumber+1);
    for i=1:2
        for n=1:sinusoidsNumber
            f(i,n)=fmax*sin(pi/(2*sinusoidsNumber)*(n-0.5));
        end
        sinusoidsNumber = sinusoidsNumber+1;
    end
end

