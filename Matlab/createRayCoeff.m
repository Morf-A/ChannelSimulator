function r = createRayCoeff(c,f,teta,Ts,K,N)
    r=zeros(2,K);
    for i=1:2
        for k=1:K
            for n=1:N(i)
                r(i,k) = r(i,k)+c(i,n)*cos(2*pi*f(i,n)*k*Ts+teta(i,n));
            end
        end
    end
end

