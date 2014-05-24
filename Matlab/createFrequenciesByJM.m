function f = createFrequenciesByJM(fmax, sinusoidsNumber)
    sinusoidsNumber = sinusoidsNumber+1;
    f = zeros(2,sinusoidsNumber);
    for i=1:2
        for n=1:sinusoidsNumber-1
            f(i,n)=fmax*cos((n*pi)/(2*sinusoidsNumber-1));
        end
        f(i,sinusoidsNumber) = fmax;
    end
end